using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("AccountLinking"));

// Identity with ApplicationUser
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

// TODO: Implement GET /api/account/identity (protected)
// Returns unified account identity information:
// - User basic info (id, email, userName)
// - All linked external providers with details
// - Authentication methods summary (hasPassword, externalProviders[])
// - Merged profile (displayName from preferred source, avatar, etc.)
// - Preferred login method if set

// TODO: Implement POST /api/account/link/initiate/{provider} (protected)
// Initiates account linking flow for logged-in users
// - Validate provider is supported (google, microsoft)
// - Challenge with the provider
// - Store current user ID in AuthenticationProperties.Items for later retrieval
// - Set redirect to /api/account/link/callback

// TODO: Implement GET /api/account/link/callback (protected)
// Handles the OAuth callback for account linking
// Requirements:
// 1. Get external login info
// 2. Extract provider key and email from claims
// 3. Check if this external account is already linked to ANOTHER user
//    - If yes, return error suggesting account merge or contact support
// 4. Check if already linked to current user
//    - If yes, return "already linked" message
// 5. Add the external login to current user using UserManager.AddLoginAsync
// 6. Merge any profile information (avatar, name) if current user fields are empty
// 7. Return success with provider details

// TODO: Implement DELETE /api/account/unlink/{provider} (protected)
// Removes external login link from current user
// CRITICAL: Must verify user won't be left without ANY login method
// Requirements:
// 1. Get current user and requested provider
// 2. Check if user has a password set (PasswordHash != null)
// 3. Count other linked external providers
// 4. If no password AND this is the last external provider:
//    - Return 400 BadRequest with clear error
// 5. Otherwise, remove the external login using RemoveLoginAsync
// 6. Return 200 with remaining authentication methods

// TODO: Implement POST /api/account/merge (protected)
// Advanced: Merge two accounts when user discovers they have duplicate accounts
// Scenario: User has local account A and Google-linked account B with same email
// Requirements:
// 1. Accept targetAccountEmail in request body
// 2. Verify current user owns both accounts (by email/password verification of target)
// 3. Merge external logins from target account to current account
// 4. Transfer any user data if applicable
// 5. Disable/delete the merged target account
// 6. Return success with merged account details

// TODO: Implement PUT /api/account/preferences (protected)
// Allow user to set preferences:
// - preferredProvider: which provider to use by default for login
// - autoLinkNewProviders: boolean to automatically link new providers with same email

// TODO: Implement GET /api/account/security-status (protected)
// Returns security analysis:
// - totalLoginMethods count
// - isTwoFactorRecommended (if only one method and it's external)
// - isWeakSecurity (if no password and only one external provider)
// - recommendations array with actionable items

app.Run();

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

// TODO: Create DTO classes for request/response
// - LinkProviderRequest
// - UnlinkProviderRequest  
// - AccountIdentityResponse
// - SecurityStatusResponse
// - MergeAccountsRequest
// - UserPreferencesRequest
