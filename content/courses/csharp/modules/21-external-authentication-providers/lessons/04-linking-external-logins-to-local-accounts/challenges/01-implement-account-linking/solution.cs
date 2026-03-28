using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("AccountLinking"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.SaveTokens = true;
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
        options.SaveTokens = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// GET /api/account/identity - Unified account view
app.MapGet("/api/account/identity", async (
    ClaimsPrincipal user,
    UserManager<ApplicationUser> userManager,
    ILogger<Program> logger) =>
{
    var dbUser = await userManager.GetUserAsync(user);
    if (dbUser == null) return Results.Unauthorized();

    var logins = await userManager.GetLoginsAsync(dbUser);
    var roles = await userManager.GetRolesAsync(dbUser);
    
    // Determine best display name
    var displayName = !string.IsNullOrEmpty(dbUser.DisplayName) 
        ? dbUser.DisplayName 
        : dbUser.Email?.Split('@')[0] ?? dbUser.UserName;

    return Results.Ok(new AccountIdentityResponse
    {
        User = new UserBasicInfo
        {
            Id = dbUser.Id,
            Email = dbUser.Email,
            UserName = dbUser.UserName,
            PhoneNumber = dbUser.PhoneNumber,
            EmailConfirmed = dbUser.EmailConfirmed,
            TwoFactorEnabled = dbUser.TwoFactorEnabled
        },
        Profile = new UserProfileInfo
        {
            DisplayName = displayName,
            AvatarUrl = dbUser.ProfilePictureUrl,
            CreatedAt = dbUser.CreatedAt,
            LastLoginAt = dbUser.LastLoginAt
        },
        AuthenticationMethods = new AuthMethodsInfo
        {
            HasPassword = !string.IsNullOrEmpty(dbUser.PasswordHash),
            ExternalProviders = logins.Select(l => new ExternalProviderInfo
            {
                Provider = l.LoginProvider,
                ProviderKey = l.ProviderKey,
                DisplayName = l.ProviderDisplayName ?? l.LoginProvider
            }).ToList(),
            TwoFactorMethods = dbUser.TwoFactorEnabled 
                ? new[] { "Authenticator" } 
                : Array.Empty<string>()
        },
        Preferences = new UserPreferencesInfo
        {
            PreferredProvider = dbUser.PreferredProvider,
            AutoLinkNewProviders = dbUser.AutoLinkNewProviders
        },
        Roles = roles.ToList()
    });
}).RequireAuthorization();

// POST /api/account/link/initiate/{provider} - Start linking flow
app.MapPost("/api/account/link/initiate/{provider}", async (
    string provider,
    HttpContext ctx,
    UserManager<ApplicationUser> userManager,
    ILogger<Program> logger) =>
{
    var validProviders = new[] { "google", "microsoft" };
    if (!validProviders.Contains(provider.ToLower()))
    {
        return Results.BadRequest(new { Error = $"Provider '{provider}' is not supported. Valid: {string.Join(", ", validProviders)}" });
    }

    var user = await userManager.GetUserAsync(ctx.User);
    if (user == null) return Results.Unauthorized();

    var scheme = provider.ToLower() switch
    {
        "google" => "Google",
        "microsoft" => "MicrosoftAccount",
        _ => throw new InvalidOperationException()
    };

    logger.LogInformation("User {UserId} initiating link with {Provider}", user.Id, provider);

    return Results.Challenge(
        new AuthenticationProperties
        {
            RedirectUri = "/api/account/link/callback",
            Items = 
            {
                { "link_user_id", user.Id },
                { "link_provider", provider }
            }
        },
        new[] { scheme }
    );
}).RequireAuthorization();

// GET /api/account/link/callback - Handle linking callback
app.MapGet("/api/account/link/callback", async (
    HttpContext ctx,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<Program> logger) =>
{
    var info = await signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
        return Results.BadRequest(new { Error = "Failed to get external login information" });
    }

    var currentUserId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var provider = info.LoginProvider;
    var providerKey = info.ProviderKey;
    var email = info.Principal.FindFirstValue(ClaimTypes.Email);

    // Check if this external account is linked to another user
    var existingUser = await userManager.FindByLoginAsync(provider, providerKey);
    if (existingUser != null && existingUser.Id != currentUserId)
    {
        logger.LogWarning("External account {Provider}:{Key} already linked to user {UserId}",
            provider, providerKey, existingUser.Id);
        
        return Results.Conflict(new
        {
            Error = "This external account is already linked to another user",
            Suggestion = "If you own both accounts, use the account merge feature or contact support",
            ExistingAccountEmail = existingUser.Email
        });
    }

    var currentUser = await userManager.GetUserAsync(ctx.User);
    if (currentUser == null) return Results.Unauthorized();

    // Check if already linked to current user
    var userLogins = await userManager.GetLoginsAsync(currentUser);
    if (userLogins.Any(l => l.LoginProvider == provider && l.ProviderKey == providerKey))
    {
        return Results.Ok(new
        {
            Message = $"{provider} is already linked to your account",
            Provider = provider,
            AlreadyLinked = true
        });
    }

    // Add the external login
    var result = await userManager.AddLoginAsync(currentUser, info);
    if (!result.Succeeded)
    {
        logger.LogError("Failed to link {Provider}: {Errors}", provider,
            string.Join(", ", result.Errors.Select(e => e.Description)));
        return Results.BadRequest(new { Error = "Failed to link account", Details = result.Errors });
    }

    // Merge profile data if empty
    if (string.IsNullOrEmpty(currentUser.DisplayName))
    {
        var name = info.Principal.FindFirstValue(ClaimTypes.Name) 
            ?? info.Principal.FindFirstValue(ClaimTypes.GivenName);
        if (!string.IsNullOrEmpty(name))
        {
            currentUser.DisplayName = name;
        }
    }

    if (string.IsNullOrEmpty(currentUser.ProfilePictureUrl))
    {
        var picture = info.Principal.FindFirstValue("picture") 
            ?? info.Principal.FindFirstValue(ClaimTypes.Uri);
        if (!string.IsNullOrEmpty(picture))
        {
            currentUser.ProfilePictureUrl = picture;
        }
    }

    await userManager.UpdateAsync(currentUser);
    logger.LogInformation("User {UserId} linked {Provider} account", currentUser.Id, provider);

    return Results.Ok(new
    {
        Message = $"Successfully linked {provider} to your account",
        Provider = provider,
        Email = email,
        LinkedAt = DateTime.UtcNow
    });
}).RequireAuthorization();

// DELETE /api/account/unlink/{provider} - Remove external login
app.MapDelete("/api/account/unlink/{provider}", async (
    string provider,
    ClaimsPrincipal user,
    UserManager<ApplicationUser> userManager,
    ILogger<Program> logger) =>
{
    var dbUser = await userManager.GetUserAsync(user);
    if (dbUser == null) return Results.Unauthorized();

    // Get all login methods
    var logins = await userManager.GetLoginsAsync(dbUser);
    var hasPassword = !string.IsNullOrEmpty(dbUser.PasswordHash);
    var otherProviders = logins.Where(l => !l.LoginProvider.Equals(provider, StringComparison.OrdinalIgnoreCase)).ToList();

    // CRITICAL: Prevent lockout
    if (!hasPassword && otherProviders.Count == 0)
    {
        return Results.BadRequest(new
        {
            Error = "Cannot unlink your only login method",
            Details = "Add a password or link another provider first to prevent account lockout",
            CurrentMethods = new { HasPassword = false, Providers = logins.Select(l => l.LoginProvider) }
        });
    }

    // Find the specific login to remove
    var loginToRemove = logins.FirstOrDefault(l => 
        l.LoginProvider.Equals(provider, StringComparison.OrdinalIgnoreCase));
    
    if (loginToRemove == null)
    {
        return Results.NotFound(new { Error = $"Provider '{provider}' is not linked to your account" });
    }

    // Remove the login
    var result = await userManager.RemoveLoginAsync(dbUser, loginToRemove.LoginProvider, loginToRemove.ProviderKey);
    if (!result.Succeeded)
    {
        logger.LogError("Failed to unlink {Provider}: {Errors}", provider,
            string.Join(", ", result.Errors.Select(e => e.Description)));
        return Results.BadRequest(new { Error = "Failed to unlink provider", Details = result.Errors });
    }

    logger.LogInformation("User {UserId} unlinked {Provider}", dbUser.Id, provider);

    // Return updated auth methods
    var updatedLogins = await userManager.GetLoginsAsync(dbUser);
    return Results.Ok(new
    {
        Message = $"Successfully unlinked {provider}",
        UnlinkedProvider = provider,
        RemainingMethods = new
        {
            HasPassword = !string.IsNullOrEmpty(dbUser.PasswordHash),
            ExternalProviders = updatedLogins.Select(l => l.LoginProvider)
        },
        Warning = updatedLogins.Count == 0 && !hasPassword 
            ? "You have no authentication methods remaining!" 
            : null
    });
}).RequireAuthorization();

// GET /api/account/security-status - Security analysis
app.MapGet("/api/account/security-status", async (
    ClaimsPrincipal user,
    UserManager<ApplicationUser> userManager,
    ILogger<Program> logger) =>
{
    var dbUser = await userManager.GetUserAsync(user);
    if (dbUser == null) return Results.Unauthorized();

    var logins = await userManager.GetLoginsAsync(dbUser);
    var hasPassword = !string.IsNullOrEmpty(dbUser.PasswordHash);
    var providerCount = logins.Count;
    var totalMethods = (hasPassword ? 1 : 0) + providerCount;

    var recommendations = new List<string>();
    
    if (totalMethods == 1)
    {
        recommendations.Add("Add a backup login method to prevent account lockout");
    }
    if (!hasPassword && providerCount > 0)
    {
        recommendations.Add("Consider setting a password as a fallback authentication method");
    }
    if (!dbUser.TwoFactorEnabled && totalMethods >= 2)
    {
        recommendations.Add("Enable two-factor authentication for enhanced security");
    }
    if (providerCount >= 2 && !dbUser.PreferredProvider.HasValue)
    {
        recommendations.Add("Set a preferred provider to streamline your login experience");
    }

    var status = new SecurityStatusResponse
    {
        TotalLoginMethods = totalMethods,
        IsSecure = totalMethods >= 2 || (hasPassword && dbUser.TwoFactorEnabled),
        IsAtRisk = totalMethods == 1,
        RiskFactors = totalMethods == 1 ? new[] { "Single point of failure" } : Array.Empty<string>(),
        Recommendations = recommendations,
        Breakdown = new AuthBreakdown
        {
            HasPassword = hasPassword,
            ExternalProviderCount = providerCount,
            TwoFactorEnabled = dbUser.TwoFactorEnabled,
            EmailConfirmed = dbUser.EmailConfirmed,
            PhoneConfirmed = dbUser.PhoneNumberConfirmed
        }
    };

    return Results.Ok(status);
}).RequireAuthorization();

// PUT /api/account/preferences - Update preferences
app.MapPut("/api/account/preferences", async (
    UserPreferencesRequest request,
    ClaimsPrincipal user,
    UserManager<ApplicationUser> userManager) =>
{
    var dbUser = await userManager.GetUserAsync(user);
    if (dbUser == null) return Results.Unauthorized();

    if (request.PreferredProvider.HasValue)
    {
        // Validate provider is linked
        var logins = await userManager.GetLoginsAsync(dbUser);
        var validProviders = logins.Select(l => l.LoginProvider).Append("Local").ToList();
        
        if (!validProviders.Contains(request.PreferredProvider.Value, StringComparer.OrdinalIgnoreCase))
        {
            return Results.BadRequest(new { Error = "Preferred provider must be a linked provider or 'Local'" });
        }
        
        dbUser.PreferredProvider = request.PreferredProvider.Value;
    }

    if (request.AutoLinkNewProviders.HasValue)
    {
        dbUser.AutoLinkNewProviders = request.AutoLinkNewProviders.Value;
    }

    await userManager.UpdateAsync(dbUser);

    return Results.Ok(new
    {
        Message = "Preferences updated",
        Preferences = new
        {
            dbUser.PreferredProvider,
            dbUser.AutoLinkNewProviders
        }
    });
}).RequireAuthorization();

app.Run();

// Entity Models
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? PreferredProvider { get; set; }
    public bool AutoLinkNewProviders { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}

// DTOs
public class AccountIdentityResponse
{
    public UserBasicInfo User { get; set; } = null!;
    public UserProfileInfo Profile { get; set; } = null!;
    public AuthMethodsInfo AuthenticationMethods { get; set; } = null!;
    public UserPreferencesInfo Preferences { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}

public class UserBasicInfo
{
    public string Id { get; set; } = null!;
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
}

public class UserProfileInfo
{
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class AuthMethodsInfo
{
    public bool HasPassword { get; set; }
    public List<ExternalProviderInfo> ExternalProviders { get; set; } = new();
    public string[] TwoFactorMethods { get; set; } = Array.Empty<string>();
}

public class ExternalProviderInfo
{
    public string Provider { get; set; } = null!;
    public string ProviderKey { get; set; } = null!;
    public string? DisplayName { get; set; }
}

public class UserPreferencesInfo
{
    public string? PreferredProvider { get; set; }
    public bool AutoLinkNewProviders { get; set; }
}

public class UserPreferencesRequest
{
    public string? PreferredProvider { get; set; }
    public bool? AutoLinkNewProviders { get; set; }
}

public class SecurityStatusResponse
{
    public int TotalLoginMethods { get; set; }
    public bool IsSecure { get; set; }
    public bool IsAtRisk { get; set; }
    public string[] RiskFactors { get; set; } = Array.Empty<string>();
    public List<string> Recommendations { get; set; } = new();
    public AuthBreakdown Breakdown { get; set; } = null!;
}

public class AuthBreakdown
{
    public bool HasPassword { get; set; }
    public int ExternalProviderCount { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneConfirmed { get; set; }
}
