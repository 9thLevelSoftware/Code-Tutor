using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ShopFlowAuth"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.SaveTokens = true;
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.AccessType = "offline"; // Get refresh token
        options.ClaimActions.MapJsonKey(ClaimTypes.Uri, "picture");
        
        options.Events.OnCreatingTicket = async context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
            logger.LogInformation("Google login for: {Email}", email);
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/google/login", () => Results.Challenge(
    new AuthenticationProperties { RedirectUri = "/auth/google/callback" },
    new[] { "Google" }
));

app.MapGet("/auth/google/callback", async (
    HttpContext ctx,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<Program> logger) =>
{
    var info = await signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
        logger.LogWarning("Failed to get external login info from Google");
        return Results.Redirect("/login-failed");
    }

    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
    var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
    var picture = info.Principal.FindFirstValue(ClaimTypes.Uri);

    // Check if user already exists
    var existingUser = await userManager.FindByEmailAsync(email!);
    
    if (existingUser != null)
    {
        // Check if Google is already linked
        var logins = await userManager.GetLoginsAsync(existingUser);
        var hasGoogleLogin = logins.Any(l => l.LoginProvider == "Google");
        
        if (!hasGoogleLogin)
        {
            // Link Google to existing account
            var addLoginResult = await userManager.AddLoginAsync(existingUser, info);
            if (!addLoginResult.Succeeded)
            {
                logger.LogError("Failed to link Google login: {Errors}",
                    string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                return Results.BadRequest("Failed to link Google account");
            }
            logger.LogInformation("Linked Google login to existing user: {Email}", email);
        }

        existingUser.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(existingUser);
        await signInManager.SignInAsync(existingUser, isPersistent: false);
        
        return hasGoogleLogin 
            ? Results.Redirect("/dashboard") 
            : Results.Redirect("/account/linked");
    }
    else
    {
        // Create new user
        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            ProfilePictureUrl = picture,
            EmailConfirmed = true, // Google already verified email
            LastLoginAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            logger.LogError("Failed to create user: {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return Results.BadRequest("Failed to create account");
        }

        var addLoginResult = await userManager.AddLoginAsync(newUser, info);
        if (!addLoginResult.Succeeded)
        {
            logger.LogError("Failed to add external login: {Errors}",
                string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
        }

        await signInManager.SignInAsync(newUser, isPersistent: false);
        logger.LogInformation("Created new user from Google login: {Email}", email);
        
        return Results.Redirect("/welcome");
    }
});

app.MapGet("/api/account/link/google", async (
    HttpContext ctx,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) =>
{
    // This is called when user is already logged in and wants to add Google
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/account/connections" },
        new[] { "Google" }
    );
}).RequireAuthorization();

app.MapGet("/api/account/external-logins", async (
    ClaimsPrincipal user,
    UserManager<ApplicationUser> userManager) =>
{
    var dbUser = await userManager.GetUserAsync(user);
    if (dbUser == null) return Results.Unauthorized();

    var logins = await userManager.GetLoginsAsync(dbUser);
    
    return Results.Ok(new
    {
        UserId = dbUser.Id,
        Email = dbUser.Email,
        HasPassword = !string.IsNullOrEmpty(dbUser.PasswordHash),
        ExternalProviders = logins.Select(l => new
        {
            Provider = l.LoginProvider,
            ProviderKey = l.ProviderKey,
            DisplayName = l.ProviderDisplayName
        }).ToList()
    });
}).RequireAuthorization();

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
