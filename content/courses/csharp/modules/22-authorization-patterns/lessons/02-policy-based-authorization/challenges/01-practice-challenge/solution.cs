using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
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

// Configure Authorization with custom policies
builder.Services.AddAuthorization(options =>
{
    // Age-based policy - user must be 18 or older
    options.AddPolicy("Age18Plus", policy =>
        policy.Requirements.Add(new AgeRequirement(18)));
    
    // Permission-based policy for editing products
    options.AddPolicy("CanEditProducts", policy =>
        policy.Requirements.Add(new PermissionRequirement("CanEditProducts")));
    
    // Combined policy - must be Admin AND 18+
    options.AddPolicy("AdminAndAdult", policy =>
    {
        policy.RequireRole("Admin");
        policy.Requirements.Add(new AgeRequirement(18));
    });
    
    // Premium customer policy
    options.AddPolicy("PremiumCustomer", policy =>
        policy.Requirements.Add(new PremiumCustomerRequirement()));
    
    // Permission-based policy for deleting products
    options.AddPolicy("CanDeleteProducts", policy =>
        policy.Requirements.Add(new PermissionRequirement("CanDeleteProducts")));
});

// Register custom authorization handlers
builder.Services.AddSingleton<IAuthorizationHandler, AgeAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, PremiumCustomerHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Age-restricted products endpoint (alcohol, mature content, etc.)
app.MapGet("/api/products/restricted", [Authorize(Policy = "Age18Plus")] (
    HttpContext context) =>
{
    var age = context.User.FindFirstValue("Age");
    
    return Results.Ok(new
    {
        Message = "Age-restricted products (18+)",
        UserAge = age,
        Products = new[]
        {
            new { Id = 101, Name = "Vintage Wine", Category = "Alcohol", Price = 45.99m, AgeRestricted = true },
            new { Id = 102, Name = "Mature Content Book", Category = "Media", Price = 19.99m, AgeRestricted = true },
            new { Id = 103, Name = "Premium Tobacco", Category = "Tobacco", Price = 29.99m, AgeRestricted = true }
        }
    });
}).WithName("RestrictedProducts").WithTags("Products");

// Create products endpoint - requires edit permission
app.MapPost("/api/products", [Authorize(Policy = "CanEditProducts")] (
    CreateProductRequest request,
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var permissions = context.User.FindFirstValue("Permission");
    
    // Simulate product creation
    var newProduct = new
    {
        Id = new Random().Next(1000, 9999),
        request.Name,
        request.Price,
        request.Category,
        CreatedBy = userId,
        CreatedAt = DateTime.UtcNow
    };
    
    return Results.Created($"/api/products/{newProduct.Id}", new
    {
        Message = "Product created successfully",
        Product = newProduct,
        GrantedBy = permissions
    });
}).WithName("CreateProduct").WithTags("Products");

// Advanced admin panel - requires Admin role AND 18+
app.MapGet("/api/admin/advanced", [Authorize(Policy = "AdminAndAdult")] (
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var role = context.User.FindFirstValue(ClaimTypes.Role);
    var age = context.User.FindFirstValue("Age");
    
    return Results.Ok(new
    {
        Message = "Advanced admin panel - sensitive operations",
        UserId = userId,
        Role = role,
        Age = age,
        SensitiveData = new
        {
            FinancialReports = new[] { "Q1-2024", "Q2-2024", "Q3-2024" },
            UserAnalytics = new { TotalUsers = 15000, ActiveUsers = 8900 },
            SecurityLogs = new[] { "Login attempts", "Failed authentications", "Suspicious activities" }
        },
        Warning = "This data is sensitive and requires both Admin role AND verified adult status"
    });
}).WithName("AdvancedAdmin").WithTags("Admin");

// Premium customer content
app.MapGet("/api/customer/premium-content", [Authorize(Policy = "PremiumCustomer")] (
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = context.User.FindFirstValue(ClaimTypes.Email);
    
    return Results.Ok(new
    {
        Message = "Exclusive premium content",
        CustomerId = userId,
        Email = email,
        Membership = "Premium",
        Benefits = new
        {
            Discounts = new[]
            {
                new { Code = "PREMIUM20", Description = "20% off all products", DiscountPercent = 20 },
                new { Code = "FLASH50", Description = "50% off flash sales", DiscountPercent = 50 }
            },
            Features = new[] { "Free shipping", "Priority support", "Early access to sales", "Extended returns" },
            LoyaltyPoints = 2500,
            NextRewardAt = 5000
        }
    });
}).WithName("PremiumContent").WithTags("Customer");

// Delete products endpoint - requires delete permission
app.MapDelete("/api/products/{id:int}", async (
    int id,
    HttpContext context,
    IAuthorizationService authorizationService) =>
{
    // Demonstrate imperative authorization check
    var result = await authorizationService.AuthorizeAsync(
        context.User, 
        null, 
        "CanDeleteProducts");
    
    if (!result.Succeeded)
    {
        return Results.Forbid();
    }
    
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var permissions = context.User.FindFirstValue("Permission");
    
    return Results.Ok(new
    {
        Message = "Product deleted successfully",
        ProductId = id,
        DeletedBy = userId,
        PermissionsUsed = permissions,
        DeletedAt = DateTime.UtcNow
    });
}).WithName("DeleteProduct").WithTags("Products");

// Public endpoint
app.MapGet("/api/public/products", () =>
{
    return Results.Ok(new
    {
        Message = "Public product catalog",
        Products = new[]
        {
            new { Id = 1, Name = "T-Shirt", Price = 29.99m, Category = "Clothing" },
            new { Id = 2, Name = "Coffee Mug", Price = 15.99m, Category = "Home" },
            new { Id = 3, Name = "Notebook", Price = 9.99m, Category = "Stationery" }
        }
    });
}).WithName("PublicProducts").WithTags("Public");

// Endpoint to check current user's claims (for debugging)
app.MapGet("/api/user/claims", [Authorize] (HttpContext context) =>
{
    var claims = context.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
    
    return Results.Ok(new
    {
        Message = "Your current claims",
        UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
        Claims = claims
    });
}).WithName("UserClaims").WithTags("User");

app.MapGet("/", () => "Policy-Based Authorization - ShopFlow API (Policy Challenge)");

app.Run();

// ==================== CUSTOM AUTHORIZATION REQUIREMENTS ====================

// Age requirement
public class AgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }
    
    public AgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}

// Permission requirement
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

// Premium customer requirement
public class PremiumCustomerRequirement : IAuthorizationRequirement
{
    // No parameters needed - checks for IsPremium claim or Premium role
}

// ==================== AUTHORIZATION HANDLERS ====================

// Handler for age-based authorization
public class AgeAuthorizationHandler : AuthorizationHandler<AgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AgeRequirement requirement)
    {
        // Get the age claim
        var ageClaim = context.User.FindFirst(c => c.Type == "Age")?.Value;
        
        if (!string.IsNullOrEmpty(ageClaim) && int.TryParse(ageClaim, out var age))
        {
            if (age >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}

// Handler for permission-based authorization
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Check for Permission claim - can contain multiple permissions separated by comma
        var permissionClaim = context.User.FindFirst(c => c.Type == "Permission")?.Value;
        
        if (!string.IsNullOrEmpty(permissionClaim))
        {
            var permissions = permissionClaim.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
            
            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
        
        // Also check for role-based permissions (e.g., Admin has all permissions)
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// Handler for premium customer authorization
public class PremiumCustomerHandler : AuthorizationHandler<PremiumCustomerRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PremiumCustomerRequirement requirement)
    {
        // Check for IsPremium claim
        var isPremiumClaim = context.User.FindFirst(c => c.Type == "IsPremium")?.Value;
        
        if (isPremiumClaim?.ToLower() == "true")
        {
            context.Succeed(requirement);
        }
        
        // Also check for Premium role as alternative
        if (context.User.IsInRole("Premium"))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// ==================== SUPPORTING CLASSES ====================

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? Age { get; set; }
    public bool IsPremium { get; set; }
}

// Request/Response records
public record CreateProductRequest(string Name, decimal Price, string Category);

// Placeholder for ApplicationDbContext
public class ApplicationDbContext { }
