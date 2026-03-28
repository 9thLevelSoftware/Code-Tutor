using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Identity with Role support
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication is configured
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Add Authorization services
builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Seed roles on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = new RoleSeeder(
        services.GetRequiredService<RoleManager<IdentityRole>>(),
        services.GetRequiredService<UserManager<ApplicationUser>>(),
        services.GetRequiredService<ILogger<RoleSeeder>>());
    await seeder.SeedAsync();
}

// Admin-only dashboard endpoint
app.MapGet("/api/admin/dashboard", [Authorize(Roles = "Admin")] (
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var role = context.User.FindFirstValue(ClaimTypes.Role);
    
    return Results.Ok(new
    {
        Message = "Admin dashboard data",
        UserId = userId,
        Role = role,
        Stats = new
        {
            TotalUsers = 150,
            TotalOrders = 1250,
            Revenue = 45000.00m,
            LastUpdated = DateTime.UtcNow
        }
    });
}).WithName("AdminDashboard").WithTags("Admin");

// User and Admin accessible profile endpoint
app.MapGet("/api/user/profile", [Authorize(Roles = "User,Admin")] (
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = context.User.FindFirstValue(ClaimTypes.Email);
    var role = context.User.FindFirstValue(ClaimTypes.Role);
    
    return Results.Ok(new
    {
        UserId = userId,
        Email = email,
        Role = role,
        Message = "User profile data"
    });
}).WithName("UserProfile").WithTags("User");

// Any authenticated user can access the catalog
app.MapGet("/api/public/catalog", [Authorize] () =>
{
    return Results.Ok(new
    {
        Products = new[]
        {
            new { Id = 1, Name = "Product A", Price = 29.99m },
            new { Id = 2, Name = "Product B", Price = 49.99m },
            new { Id = 3, Name = "Product C", Price = 19.99m }
        }
    });
}).WithName("PublicCatalog").WithTags("Public");

// Admin-only role assignment endpoint
app.MapPost("/api/admin/users/assign-role", async (
    AssignRoleRequest request,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) =>
{
    // Check if the role exists
    if (!await roleManager.RoleExistsAsync(request.Role))
    {
        return Results.BadRequest(new { Error = $"Role '{request.Role}' does not exist" });
    }
    
    // Find the user
    var user = await userManager.FindByIdAsync(request.UserId);
    if (user == null)
    {
        return Results.NotFound(new { Error = "User not found" });
    }
    
    // Check if user already has the role
    var hasRole = await userManager.IsInRoleAsync(user, request.Role);
    if (hasRole)
    {
        return Results.BadRequest(new { Error = "User already has this role" });
    }
    
    // Assign the role
    var result = await userManager.AddToRoleAsync(user, request.Role);
    
    if (!result.Succeeded)
    {
        return Results.BadRequest(new { Errors = result.Errors });
    }
    
    return Results.Ok(new
    {
        Message = $"Role '{request.Role}' assigned to user '{user.Email}' successfully",
        UserId = user.Id,
        Role = request.Role
    });
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
.WithName("AssignRole")
.WithTags("Admin");

app.MapGet("/", () => "Role-Based Access Control - ShopFlow API");

app.Run();

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// Role Seeder class
public class RoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RoleSeeder> _logger;
    
    public RoleSeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<RoleSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }
    
    public async Task SeedAsync()
    {
        // Ensure roles exist
        string[] roles = { "Admin", "User", "Guest" };
        
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Created role: {Role}", role);
            }
        }
        
        // Create default admin user
        var adminEmail = "admin@shopflow.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true
            };
            
            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                _logger.LogInformation("Created default admin user: {Email}", adminEmail);
            }
            else
            {
                _logger.LogError("Failed to create admin user: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}

// Request/Response records
public record AssignRoleRequest(string UserId, string Role);

// Placeholder for ApplicationDbContext
public class ApplicationDbContext { }
