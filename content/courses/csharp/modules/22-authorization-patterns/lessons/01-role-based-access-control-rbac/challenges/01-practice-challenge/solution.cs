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
    HttpContext context,
    ILogger<Program> logger) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = context.User.FindFirstValue(ClaimTypes.Email);
    var role = context.User.FindFirstValue(ClaimTypes.Role);
    
    logger.LogInformation("Admin dashboard accessed by {Email}", email);
    
    return Results.Ok(new
    {
        Message = "Admin dashboard",
        UserId = userId,
        Email = email,
        Role = role,
        Stats = new
        {
            TotalUsers = 150,
            TotalOrders = 1250,
            TotalRevenue = 45000.00m,
            PendingOrders = 23,
            LastUpdated = DateTime.UtcNow
        }
    });
}).WithName("AdminDashboard").WithTags("Admin");

// Manager and Admin accessible inventory endpoint
app.MapGet("/api/manager/inventory", [Authorize(Roles = "Admin,Manager")] (
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var role = context.User.FindFirstValue(ClaimTypes.Role);
    
    return Results.Ok(new
    {
        Message = "Inventory management",
        UserId = userId,
        Role = role,
        Products = new[]
        {
            new { Id = 1, Name = "Laptop", Stock = 45, Price = 999.99m },
            new { Id = 2, Name = "Mouse", Stock = 3, Price = 29.99m },
            new { Id = 3, Name = "Keyboard", Stock = 25, Price = 79.99m }
        },
        LowStockAlerts = new[]
        {
            new { ProductId = 2, Name = "Mouse", CurrentStock = 3, Threshold = 10 }
        }
    });
}).WithName("InventoryManagement").WithTags("Manager");

// Customer and Admin accessible orders endpoint
app.MapGet("/api/customer/orders", [Authorize(Roles = "Customer,Admin")] (
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = context.User.FindFirstValue(ClaimTypes.Email);
    
    return Results.Ok(new
    {
        Message = "Customer orders",
        CustomerId = userId,
        Email = email,
        Orders = new[]
        {
            new { Id = 101, Total = 149.99m, Status = "Delivered", Date = DateTime.UtcNow.AddDays(-5) },
            new { Id = 102, Total = 299.99m, Status = "Processing", Date = DateTime.UtcNow.AddDays(-1) }
        }
    });
}).WithName("CustomerOrders").WithTags("Customer");

// Admin-only user promotion endpoint
app.MapPost("/api/admin/users/promote", async (
    PromoteUserRequest request,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<Program> logger) =>
{
    // Validate the target role exists
    if (!await roleManager.RoleExistsAsync(request.NewRole))
    {
        return Results.BadRequest(new { Error = $"Role '{request.NewRole}' does not exist" });
    }
    
    // Only allow promotion to Manager role through this endpoint
    if (request.NewRole != "Manager")
    {
        return Results.BadRequest(new { Error = "This endpoint only supports promoting to Manager role" });
    }
    
    // Find the user
    var user = await userManager.FindByIdAsync(request.UserId);
    if (user == null)
    {
        return Results.NotFound(new { Error = "User not found" });
    }
    
    // Check if user already has the role
    var hasRole = await userManager.IsInRoleAsync(user, request.NewRole);
    if (hasRole)
    {
        return Results.BadRequest(new { Error = $"User already has the '{request.NewRole}' role" });
    }
    
    // Assign the role
    var result = await userManager.AddToRoleAsync(user, request.NewRole);
    
    if (!result.Succeeded)
    {
        var errors = result.Errors.Select(e => e.Description).ToList();
        return Results.BadRequest(new { Errors = errors });
    }
    
    logger.LogInformation("User {UserId} promoted to {Role} by admin", user.Id, request.NewRole);
    
    return Results.Ok(new
    {
        Message = $"User promoted to '{request.NewRole}' successfully",
        UserId = user.Id,
        Email = user.Email,
        NewRole = request.NewRole,
        PromotedAt = DateTime.UtcNow
    });
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
.WithName("PromoteUser")
.WithTags("Admin");

// Admin-only product deletion endpoint
app.MapDelete("/api/admin/products/{id:int}", [Authorize(Roles = "Admin")] (
    int id,
    HttpContext context,
    ILogger<Program> logger) =>
{
    var adminEmail = context.User.FindFirstValue(ClaimTypes.Email);
    
    // Simulate product deletion
    logger.LogInformation("Product {ProductId} deleted by {Admin}", id, adminEmail);
    
    return Results.Ok(new
    {
        Message = "Product deleted successfully",
        ProductId = id,
        DeletedBy = adminEmail,
        DeletedAt = DateTime.UtcNow
    });
}).WithName("DeleteProduct").WithTags("Admin");

// Public endpoint - accessible without authentication
app.MapGet("/api/public/products", () =>
{
    return Results.Ok(new
    {
        Message = "Public product catalog",
        Products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 999.99m, InStock = true },
            new { Id = 2, Name = "Mouse", Price = 29.99m, InStock = true },
            new { Id = 3, Name = "Keyboard", Price = 79.99m, InStock = true }
        }
    });
}).WithName("PublicProducts").WithTags("Public");

app.MapGet("/", () => "Role-Based Access Control - ShopFlow API (RBAC Challenge)");

app.Run();

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
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
        string[] roles = { "Admin", "Manager", "Customer" };
        
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created role: {Role}", role);
                }
            }
        }
        
        // Create default admin user
        await CreateUserAsync(
            "admin@shopflow.com",
            "Admin",
            "System",
            "Admin123!",
            "Admin");
        
        // Create test manager user
        await CreateUserAsync(
            "manager@shopflow.com",
            "Test",
            "Manager",
            "Manager123!",
            "Manager");
        
        // Create test customer user
        await CreateUserAsync(
            "customer@shopflow.com",
            "Test",
            "Customer",
            "Customer123!",
            "Customer");
    }
    
    private async Task CreateUserAsync(string email, string firstName, string lastName, string password, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };
            
            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation("Created {Role} user: {Email}", role, email);
            }
            else
            {
                _logger.LogError("Failed to create {Role} user: {Errors}", 
                    role, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}

// Request/Response records
public record PromoteUserRequest(string UserId, string NewRole);

// Placeholder for ApplicationDbContext
public class ApplicationDbContext { }
