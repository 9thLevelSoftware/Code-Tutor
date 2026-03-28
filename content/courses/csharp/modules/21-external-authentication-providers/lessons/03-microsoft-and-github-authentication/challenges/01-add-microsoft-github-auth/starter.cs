using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MultiProviderAuth"));

// Identity is already configured
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// TODO: Add Microsoft Authentication
// Configure Microsoft Account OAuth:
// - ClientId from Configuration["Authentication:Microsoft:ClientId"]
// - ClientSecret from Configuration["Authentication:Microsoft:ClientSecret"]
// - SaveTokens = true
// - Add scopes: openid, email, profile, User.Read
// - Tenant = "common" (allows any Microsoft account - personal or work/school)

// TODO: Add GitHub Authentication
// Configure GitHub OAuth (use Microsoft.AspNetCore.Authentication.GitHub package or generic OAuth):
// - ClientId from Configuration["Authentication:GitHub:ClientId"]
// - ClientSecret from Configuration["Authentication:GitHub:ClientSecret"]
// - AuthorizationEndpoint = "https://github.com/login/oauth/authorize"
// - TokenEndpoint = "https://github.com/login/oauth/access_token"
// - UserInformationEndpoint = "https://api.github.com/user"
// - SaveTokens = true
// - Add scopes: read:user, user:email
// - Map claims: login -> ClaimTypes.Name, email -> ClaimTypes.Email, avatar_url -> ProfileImage
// - OnCreatingTicket: fetch additional emails if primary email is null

builder.Services.AddAuthentication()
    .AddCookie()
    // TODO: Add Microsoft
    // TODO: Add GitHub
;

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints already provided:
// - POST /api/auth/login (local login with email/password)
// - POST /api/auth/register (local registration)
// - GET /api/auth/me (current user info)

// TODO: Create GET /api/auth/providers
// Returns a list of available authentication providers with their display names and icons
// [{ "name": "Microsoft", "displayName": "Microsoft Account", "icon": "microsoft" }, ...]

// TODO: Create GET /api/auth/challenge/{provider}
// Initiates OAuth challenge for specified provider (microsoft or github)
// Query parameter: returnUrl (where to redirect after success)
// Should validate provider name and return 400 if invalid

// TODO: Create GET /api/auth/callback/{provider}
// Handles OAuth callback for both providers
// Requirements:
// 1. Get external login info using SignInManager
// 2. If null, redirect to /auth/error?reason=callback_failed
// 3. Extract email from claims (different claim types for each provider)
// 4. Find existing user by email
// 5. If exists and not linked: link external login, sign in, update LastLoginAt
// 6. If doesn't exist: create new user with provider info, sign in
// 7. If exists and linked: sign in normally
// 8. Redirect to returnUrl from AuthenticationProperties or /dashboard

// TODO: Create POST /api/account/link/{provider} (protected)
// Allows logged-in users to link additional providers
// Returns challenge with redirect back to /account/connections

// TODO: Create GET /api/account/suggested-providers (protected)
// Returns providers NOT yet linked to current user
// Useful for showing "Add another login method" options

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
