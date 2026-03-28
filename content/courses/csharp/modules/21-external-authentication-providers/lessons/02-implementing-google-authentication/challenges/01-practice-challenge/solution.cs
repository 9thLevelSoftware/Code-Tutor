using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configure Authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/auth/google/login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.SaveTokens = true;
    
    // Request OpenID Connect scopes
    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("profile");
    
    // Map Google picture claim to ProfileImage
    options.ClaimActions.MapJsonKey("ProfileImage", "picture");
    
    // Log successful authentications
    options.Events.OnCreatingTicket = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
        logger.LogInformation("User authenticated via Google: {Email}", email);
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// In-memory user store for this challenge
var users = new Dictionary<string, ShopFlowUser>();

// Google Login - Initiates OAuth challenge
app.MapGet("/auth/google/login", (HttpContext ctx, string? returnUrl) =>
{
    var properties = new AuthenticationProperties
    {
        RedirectUri = "/auth/google/callback",
        Items = { { "returnUrl", returnUrl ?? "/dashboard" } }
    };
    
    return Results.Challenge(properties, new[] { GoogleDefaults.AuthenticationScheme });
});

// Google Callback - Handles OAuth response
app.MapGet("/auth/google/callback", async (HttpContext ctx, ILogger<Program> logger) =>
{
    // Authenticate with Google to get external principal
    var result = await ctx.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    
    if (!result.Succeeded || result.Principal == null)
    {
        logger.LogWarning("Google authentication failed: {FailureMessage}", result.Failure?.Message);
        return Results.Redirect("/login-failed?reason=google_auth_failed");
    }
    
    // Check for OAuth errors
    if (ctx.Request.Query.ContainsKey("error"))
    {
        var error = ctx.Request.Query["error"].ToString();
        var errorDesc = ctx.Request.Query["error_description"].ToString();
        logger.LogWarning("Google OAuth error: {Error} - {Description}", error, errorDesc);
        return Results.Redirect($"/login-failed?reason={error}");
    }
    
    // Extract claims from Google
    var principal = result.Principal;
    var email = principal.FindFirstValue(ClaimTypes.Email);
    var name = principal.FindFirstValue(ClaimTypes.Name);
    var givenName = principal.FindFirstValue(ClaimTypes.GivenName);
    var familyName = principal.FindFirstValue(ClaimTypes.Surname);
    var profileImage = principal.FindFirstValue("ProfileImage");
    var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (string.IsNullOrEmpty(email))
    {
        logger.LogError("Google authentication returned no email");
        return Results.Redirect("/login-failed?reason=no_email");
    }
    
    // Get the access token for API calls
    var accessToken = await ctx.GetTokenAsync(GoogleDefaults.AuthenticationScheme, "access_token");
    
    // Create or update user
    if (!users.TryGetValue(email, out var user))
    {
        user = new ShopFlowUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };
        users[email] = user;
        logger.LogInformation("Created new user: {Email}", email);
    }
    
    // Update user info from Google
    user.Name = name ?? $"{givenName} {familyName}".Trim();
    user.ProfileImageUrl = profileImage;
    user.GoogleId = googleId;
    user.LastLoginAt = DateTime.UtcNow;
    user.GoogleAccessToken = accessToken;
    
    logger.LogInformation("User logged in: {Email} (Google ID: {GoogleId})", email, googleId);
    
    // Create identity for cookie authentication
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Name, user.Name ?? email),
        new Claim("ProfileImage", profileImage ?? ""),
        new Claim("Provider", "Google"),
        new Claim("LoginTime", DateTime.UtcNow.ToString("O"))
    };
    
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties
    {
        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
    };
    
    // Sign in with cookie scheme
    await ctx.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity),
        authProperties);
    
    // Sign out from external scheme
    await ctx.SignOutAsync(GoogleDefaults.AuthenticationScheme);
    
    // Get return URL from properties or default to /dashboard
    var returnUrl = result.Properties?.Items.TryGetValue("returnUrl", out var url) == true 
        ? url 
        : "/dashboard";
    
    return Results.Redirect(returnUrl);
});

// User Profile - Protected endpoint
app.MapGet("/api/user/profile", async (HttpContext ctx, ILogger<Program> logger) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? true)
    {
        return Results.Unauthorized();
    }
    
    var email = ctx.User.FindFirstValue(ClaimTypes.Email);
    var name = ctx.User.FindFirstValue(ClaimTypes.Name);
    var profileImage = ctx.User.FindFirstValue("ProfileImage");
    var provider = ctx.User.FindFirstValue("Provider");
    var loginTime = ctx.User.FindFirstValue("LoginTime");
    
    // Get the Google access token from cookie properties (if available)
    var authResult = await ctx.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    var accessToken = authResult.Properties?.GetTokenValue("access_token");
    
    var tokenPreview = !string.IsNullOrEmpty(accessToken) 
        ? $"{accessToken[..Math.Min(10, accessToken.Length)]}..."
        : "Not available";
    
    return Results.Ok(new UserProfileResponse(
        email ?? "unknown",
        name ?? email ?? "unknown",
        profileImage,
        provider ?? "Unknown",
        loginTime != null ? DateTime.Parse(loginTime) : null,
        tokenPreview
    ));
}).RequireAuthorization();

// Logout
app.MapPost("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

// Health check
app.MapGet("/", () => Results.Ok(new { 
    Message = "ShopFlow with Google Auth", 
    LoginUrl = "/auth/google/login",
    ProfileUrl = "/api/user/profile (requires auth)"
}));

app.Run();

// User Model
public class ShopFlowUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? GoogleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? GoogleAccessToken { get; set; }
}

// API Response Record
public record UserProfileResponse(
    string Email,
    string Name,
    string? ProfileImage,
    string Provider,
    DateTime? LoginTime,
    string AccessTokenPreview
);
