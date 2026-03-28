using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Authentication with multiple providers
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.ClaimActions.MapJsonKey("ProfileImage", "picture");
})
.AddMicrosoftAccount(options =>
{
    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
    options.Tenant = "common"; // Allows personal and work/school accounts
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.Scope.Add("User.Read");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
    options.ClaimActions.MapJsonKey("ProfileImage", "photo");
})
.AddOAuth("GitHub", "GitHub", options =>
{
    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
    options.UserInformationEndpoint = "https://api.github.com/user";
    options.CallbackPath = "/signin-github";
    options.SaveTokens = true;
    options.Scope.Add("read:user");
    options.Scope.Add("user:email");
    
    // Map GitHub claims
    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
    options.ClaimActions.MapJsonKey("ProfileImage", "avatar_url");
    options.ClaimActions.MapJsonKey("ProfileUrl", "html_url");
    
    // Handle GitHub's email behavior (private emails require separate call)
    options.Events.OnCreatingTicket = async context =>
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
        request.Headers.Add("User-Agent", "ShopFlowApp/1.0"); // Required by GitHub
        
        using var result = await context.Backchannel.SendAsync(request);
        var user = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
        context.RunClaimActions(user.RootElement);
        
        // Check if email is private and fetch from emails endpoint
        var email = user.RootElement.GetProperty("email").GetString();
        if (string.IsNullOrEmpty(email))
        {
            using var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
            emailRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
            emailRequest.Headers.Add("User-Agent", "ShopFlowApp/1.0");
            
            using var emailResult = await context.Backchannel.SendAsync(emailRequest);
            var emails = JsonDocument.Parse(await emailResult.Content.ReadAsStringAsync());
            
            var primaryEmail = emails.RootElement.EnumerateArray()
                .FirstOrDefault(e => e.GetProperty("primary").GetBoolean())
                .GetProperty("email").GetString();
            
            if (!string.IsNullOrEmpty(primaryEmail))
            {
                context.Identity?.AddClaim(new Claim(ClaimTypes.Email, primaryEmail));
            }
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// User store with multiple provider support
var users = new Dictionary<string, ShopFlowUser>();

// Provider scheme mapping
var providerSchemes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["google"] = "Google",
    ["microsoft"] = MicrosoftAccountDefaults.AuthenticationScheme,
    ["github"] = "GitHub"
};

var providerMetadata = new[]
{
    new { Name = "google", DisplayName = "Google", Icon = "google", Color = "#4285F4" },
    new { Name = "microsoft", DisplayName = "Microsoft Account", Icon = "microsoft", Color = "#2F2F2F" },
    new { Name = "github", DisplayName = "GitHub", Icon = "github", Color = "#24292E" }
};

// Get available providers
app.MapGet("/api/auth/providers", () => Results.Ok(providerMetadata));

// Generic challenge endpoint
app.MapGet("/api/auth/challenge/{provider}", (string provider, string? returnUrl) =>
{
    if (!providerSchemes.ContainsKey(provider))
    {
        var validProviders = string.Join(", ", providerSchemes.Keys);
        return Results.BadRequest(new { Error = $"Invalid provider '{provider}'. Valid: {validProviders}" });
    }
    
    var scheme = providerSchemes[provider];
    var properties = new AuthenticationProperties
    {
        RedirectUri = $"/api/auth/callback/{provider}",
        Items = { { "returnUrl", returnUrl ?? "/dashboard" }, { "provider", provider } }
    };
    
    return Results.Challenge(properties, new[] { scheme });
});

// Generic callback handler for all providers
app.MapGet("/api/auth/callback/{provider}", async (
    string provider,
    HttpContext ctx,
    ILogger<Program> logger) =>
{
    if (!providerSchemes.TryGetValue(provider, out var scheme))
    {
        return Results.BadRequest(new { Error = $"Unknown provider: {provider}" });
    }
    
    // Check for OAuth errors
    if (ctx.Request.Query.TryGetValue("error", out var error))
    {
        var errorDesc = ctx.Request.Query["error_description"].ToString();
        logger.LogWarning("OAuth error from {Provider}: {Error} - {Description}", provider, error, errorDesc);
        return Results.Redirect($"/login-failed?reason={error}&provider={provider}");
    }
    
    // Authenticate with the provider
    var result = await ctx.AuthenticateAsync(scheme);
    
    if (!result.Succeeded || result.Principal == null)
    {
        logger.LogWarning("Authentication failed for {Provider}: {Failure}", provider, result.Failure?.Message);
        return Results.Redirect($"/login-failed?provider={provider}");
    }
    
    // Extract claims - different providers may have different claim types
    var principal = result.Principal;
    var email = principal.FindFirstValue(ClaimTypes.Email);
    var name = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue(ClaimTypes.GivenName);
    var profileImage = principal.FindFirstValue("ProfileImage");
    var providerKey = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (string.IsNullOrEmpty(email))
    {
        logger.LogError("No email claim from {Provider}", provider);
        return Results.Redirect($"/login-failed?reason=no_email&provider={provider}");
    }
    
    // Get access token
    var accessToken = await ctx.GetTokenAsync(scheme, "access_token");
    var refreshToken = await ctx.GetTokenAsync(scheme, "refresh_token");
    var expiresIn = await ctx.GetTokenAsync(scheme, "expires_at");
    
    // Find or create user
    if (!users.TryGetValue(email, out var user))
    {
        user = new ShopFlowUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = name ?? email.Split('@')[0],
            ProfileImageUrl = profileImage,
            PrimaryProvider = provider,
            CreatedAt = DateTime.UtcNow
        };
        users[email] = user;
        logger.LogInformation("Created new user {Email} via {Provider}", email, provider);
    }
    else
    {
        user.Name = name ?? user.Name;
        user.ProfileImageUrl = profileImage ?? user.ProfileImageUrl;
        logger.LogInformation("User {Email} logged in via {Provider}", email, provider);
    }
    
    // Update or add external login
    var existingLogin = user.ExternalLogins.FirstOrDefault(l => l.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase));
    if (existingLogin != null)
    {
        existingLogin.AccessToken = accessToken;
        existingLogin.RefreshToken = refreshToken;
        existingLogin.UpdatedAt = DateTime.UtcNow;
    }
    else
    {
        user.ExternalLogins.Add(new ExternalLogin
        {
            Provider = provider,
            ProviderKey = providerKey ?? email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AddedAt = DateTime.UtcNow
        });
    }
    
    user.LastLoginAt = DateTime.UtcNow;
    
    // Create cookie identity
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim("PrimaryProvider", user.PrimaryProvider),
        new Claim("LinkedProviders", string.Join(",", user.ExternalLogins.Select(l => l.Provider)))
    };
    
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = true });
    
    // Sign out from external scheme
    await ctx.SignOutAsync(scheme);
    
    var returnUrl = result.Properties?.Items.TryGetValue("returnUrl", out var url) == true 
        ? url 
        : "/dashboard";
    
    return Results.Redirect(returnUrl);
});

// Get linked providers (protected)
app.MapGet("/api/user/linked-providers", (HttpContext ctx) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? true)
    {
        return Results.Unauthorized();
    }
    
    var email = ctx.User.FindFirstValue(ClaimTypes.Email);
    if (!users.TryGetValue(email, out var user))
    {
        return Results.NotFound();
    }
    
    return Results.Ok(new
    {
        Email = user.Email,
        PrimaryProvider = user.PrimaryProvider,
        LinkedProviders = user.ExternalLogins.Select(l => new
        {
            l.Provider,
            l.AddedAt,
            HasRefreshToken = !string.IsNullOrEmpty(l.RefreshToken)
        }),
        AvailableProviders = providerMetadata.Select(p => p.Name)
            .Except(user.ExternalLogins.Select(l => l.Provider.ToLower()))
    });
}).RequireAuthorization();

// Set primary provider (protected)
app.MapPost("/api/user/set-primary-provider/{provider}", (string provider, HttpContext ctx, ILogger<Program> logger) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? true)
    {
        return Results.Unauthorized();
    }
    
    var email = ctx.User.FindFirstValue(ClaimTypes.Email);
    if (!users.TryGetValue(email, out var user))
    {
        return Results.NotFound();
    }
    
    // Verify provider is linked
    if (!user.ExternalLogins.Any(l => l.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase)))
    {
        return Results.BadRequest(new { Error = $"Provider '{provider}' is not linked to your account" });
    }
    
    user.PrimaryProvider = provider;
    logger.LogInformation("User {Email} set primary provider to {Provider}", email, provider);
    
    return Results.Ok(new { Message = $"Primary provider set to {provider}", PrimaryProvider = provider });
}).RequireAuthorization();

// Logout
app.MapPost("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.Run();

// Models
public class ShopFlowUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string PrimaryProvider { get; set; } = "local";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<ExternalLogin> ExternalLogins { get; set; } = new();
}

public class ExternalLogin
{
    public string Provider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
