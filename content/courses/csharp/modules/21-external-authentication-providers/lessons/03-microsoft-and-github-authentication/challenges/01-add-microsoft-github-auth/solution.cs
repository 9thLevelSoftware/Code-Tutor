using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MultiProviderAuth"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddCookie()
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
        options.SaveTokens = true;
        options.Scope.Add("openid");
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.Scope.Add("User.Read");
        options.Tenant = "common"; // Allows both personal and work/school accounts
        
        options.Events.OnCreatingTicket = async context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
            logger.LogInformation("Microsoft login for: {Email}", email);
        };
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
        
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        options.ClaimActions.MapJsonKey("ProfileImage", "avatar_url");
        options.ClaimActions.MapJsonKey("ProfileUrl", "html_url");
        
        options.Events.OnCreatingTicket = async context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
            request.Headers.Add("User-Agent", "ShopFlowApp/1.0");
            
            var result = await context.Backchannel.SendAsync(request);
            var user = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
            
            context.RunClaimActions(user.RootElement);
            
            // GitHub may not return email in user endpoint if it's private
            // Fetch from emails endpoint
            var email = user.RootElement.GetProperty("email").GetString();
            if (string.IsNullOrEmpty(email))
            {
                var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                emailRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
                emailRequest.Headers.Add("User-Agent", "ShopFlowApp/1.0");
                
                var emailResult = await context.Backchannel.SendAsync(emailRequest);
                var emails = JsonDocument.Parse(await emailResult.Content.ReadAsStringAsync());
                
                var primaryEmail = emails.RootElement.EnumerateArray()
                    .FirstOrDefault(e => e.GetProperty("primary").GetBoolean())
                    .GetProperty("email").GetString();
                
                if (!string.IsNullOrEmpty(primaryEmail))
                {
                    context.Identity?.AddClaim(new Claim(ClaimTypes.Email, primaryEmail));
                }
            }
            
            var username = context.Principal?.FindFirstValue(ClaimTypes.Name);
            logger.LogInformation("GitHub login for: {Username}", username);
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Helper to get display name for provider
static string GetProviderDisplayName(string provider) => provider.ToLower() switch
{
    "microsoft" => "Microsoft Account",
    "github" => "GitHub",
    _ => provider
};

app.MapGet("/api/auth/providers", () => Results.Ok(new[]
{
    new { Name = "Microsoft", DisplayName = "Microsoft Account", Icon = "microsoft", Color = "#2F2F2F" },
    new { Name = "GitHub", DisplayName = "GitHub", Icon = "github", Color = "#24292E" }
}));

app.MapGet("/api/auth/challenge/{provider}", (string provider, string? returnUrl) =>
{
    var validProviders = new[] { "microsoft", "github" };
    if (!validProviders.Contains(provider.ToLower()))
    {
        return Results.BadRequest(new { Error = $"Invalid provider: {provider}. Valid providers: {string.Join(", ", validProviders)}" });
    }
    
    var scheme = provider.ToLower() switch
    {
        "microsoft" => MicrosoftAccountDefaults.AuthenticationScheme,
        "github" => "GitHub",
        _ => throw new InvalidOperationException("Unknown provider")
    };
    
    return Results.Challenge(
        new AuthenticationProperties 
        { 
            RedirectUri = $"/api/auth/callback/{provider}?returnUrl={Uri.EscapeDataString(returnUrl ?? "/dashboard")}",
            Items = { { "provider", provider }, { "returnUrl", returnUrl ?? "/dashboard" } }
        },
        new[] { scheme }
    );
});

app.MapGet("/api/auth/callback/{provider}", async (
    string provider,
    HttpContext ctx,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<Program> logger) =>
{
    var info = await signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
        logger.LogError("External login info is null for provider: {Provider}", provider);
        return Results.Redirect("/auth/error?reason=callback_failed");
    }

    // Extract claims (different providers use different claim types)
    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    var name = provider.ToLower() switch
    {
        "microsoft" => info.Principal.FindFirstValue(ClaimTypes.Name),
        "github" => info.Principal.FindFirstValue(ClaimTypes.Name),
        _ => info.Principal.FindFirstValue(ClaimTypes.Name)
    };
    var profileImage = provider.ToLower() switch
    {
        "microsoft" => info.Principal.FindFirstValue("picture"),
        "github" => info.Principal.FindFirstValue("ProfileImage"),
        _ => null
    };

    // GitHub might have email in a different location
    if (string.IsNullOrEmpty(email) && provider.ToLower() == "github")
    {
        email = info.Principal.FindFirstValue("urn:github:email");
    }

    if (string.IsNullOrEmpty(email))
    {
        logger.LogError("Email not provided by {Provider}", provider);
        return Results.Redirect("/auth/error?reason=no_email");
    }

    var existingUser = await userManager.FindByEmailAsync(email);
    var returnUrl = ctx.Request.Query["returnUrl"].FirstOrDefault() ?? "/dashboard";

    if (existingUser != null)
    {
        var logins = await userManager.GetLoginsAsync(existingUser);
        var hasProviderLogin = logins.Any(l => l.LoginProvider.Equals(provider, StringComparison.OrdinalIgnoreCase));
        
        if (!hasProviderLogin)
        {
            var addResult = await userManager.AddLoginAsync(existingUser, info);
            if (!addResult.Succeeded)
            {
                logger.LogError("Failed to link {Provider}: {Errors}", provider, 
                    string.Join(", ", addResult.Errors.Select(e => e.Description)));
                return Results.Redirect($"/auth/error?reason=link_failed&provider={provider}");
            }
            logger.LogInformation("Linked {Provider} to existing user: {Email}", provider, email);
        }

        existingUser.LastLoginAt = DateTime.UtcNow;
        if (profileImage != null) existingUser.ProfileImageUrl = profileImage;
        await userManager.UpdateAsync(existingUser);
        await signInManager.SignInAsync(existingUser, isPersistent: false);
        
        return Results.Redirect(returnUrl);
    }
    else
    {
        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = name,
            ProfileImageUrl = profileImage,
            EmailConfirmed = true,
            LastLoginAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            logger.LogError("Failed to create user: {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return Results.Redirect("/auth/error?reason=create_failed");
        }

        await userManager.AddLoginAsync(newUser, info);
        await signInManager.SignInAsync(newUser, isPersistent: false);
        logger.LogInformation("Created new user from {Provider}: {Email}", provider, email);
        
        return Results.Redirect(returnUrl);
    }
});

app.MapPost("/api/account/link/{provider}", async (
    string provider,
    HttpContext ctx,
    string? returnUrl) =>
{
    var validProviders = new[] { "microsoft", "github" };
    if (!validProviders.Contains(provider.ToLower()))
    {
        return Results.BadRequest(new { Error = $"Invalid provider: {provider}" });
    }

    var scheme = provider.ToLower() switch
    {
        "microsoft" => MicrosoftAccountDefaults.AuthenticationScheme,
        "github" => "GitHub",
        _ => throw new InvalidOperationException("Unknown provider")
    };

    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = returnUrl ?? "/account/connections" },
        new[] { scheme }
    );
}).RequireAuthorization();

app.MapGet("/api/account/suggested-providers", async (
    ClaimsPrincipal user,
    UserManager<ApplicationUser> userManager) =>
{
    var dbUser = await userManager.GetUserAsync(user);
    if (dbUser == null) return Results.Unauthorized();

    var logins = await userManager.GetLoginsAsync(dbUser);
    var linkedProviders = logins.Select(l => l.LoginProvider.ToLower()).ToHashSet();
    
    var allProviders = new[] { "microsoft", "github" };
    var suggested = allProviders.Where(p => !linkedProviders.Contains(p)).Select(p => new
    {
        Name = p,
        DisplayName = GetProviderDisplayName(p)
    });
    
    return Results.Ok(new
    {
        LinkedProviders = logins.Select(l => l.LoginProvider),
        SuggestedProviders = suggested,
        HasPassword = !string.IsNullOrEmpty(dbUser.PasswordHash)
    });
}).RequireAuthorization();

app.Run();

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
