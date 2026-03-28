using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext is pre-configured
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ShopFlowAuth"));

// TODO: Configure Identity
// 1. Add Identity with ApplicationUser and IdentityRole
// 2. Add EntityFrameworkStores with ApplicationDbContext
// 3. Configure password policy (minimum 8 chars, require digit, require uppercase)
// 4. Set RequireConfirmedEmail = false for easier testing

// TODO: Add Authentication
// 1. Add Cookie authentication as default scheme
// 2. Add Google authentication:
//    - ClientId from Configuration["Authentication:Google:ClientId"]
//    - ClientSecret from Configuration["Authentication:Google:ClientSecret"]
//    - SaveTokens = true
//    - Add scopes: email, profile
//    - Set AccessType = "offline" to get refresh token
//    - Map the Google 'picture' claim to ClaimTypes.Uri for avatar

// TODO: Add Authorization services

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// TODO: Implement /auth/google/login endpoint
// This should Challenge with Google scheme, redirect to /auth/google/callback

// TODO: Implement /auth/google/callback endpoint
// This is where Google redirects after authentication
// Requirements:
// 1. Get the external login info using SignInManager
// 2. If no info, redirect to /login-failed
// 3. Find user by email from Google claims
// 4. If user exists but no Google login linked:
//    - Link the external login to existing account
//    - Sign in the user
//    - Redirect to /account/linked
// 5. If user doesn't exist:
//    - Create new ApplicationUser with email from Google
//    - Add Google external login
//    - Sign in the user
//    - Redirect to /welcome
// 6. If user exists and Google already linked:
//    - Sign in normally
//    - Redirect to /dashboard

// TODO: Implement /api/account/link/google endpoint (for existing logged-in users)
// This allows users to link Google to their existing local account

// TODO: Implement GET /api/account/external-logins (protected)
// Returns list of linked external providers for current user

app.Run();

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
