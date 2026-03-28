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

// Configure Authorization with resource-based policies
builder.Services.AddAuthorization(options =>
{
    // Order policies
    options.AddPolicy("CanViewOrder", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement(ResourceType.Order, Operation.Read)));
    options.AddPolicy("CanUpdateOrder", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement(ResourceType.Order, Operation.Update)));
    options.AddPolicy("CanDeleteOrder", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement(ResourceType.Order, Operation.Delete)));
    
    // Product policies
    options.AddPolicy("CanUpdateProduct", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement(ResourceType.Product, Operation.Update)));
    options.AddPolicy("CanDeleteProduct", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement(ResourceType.Product, Operation.Delete)));
    
    // User profile policies
    options.AddPolicy("CanViewProfile", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement(ResourceType.UserProfile, Operation.Read)));
    options.AddPolicy("CanUpdateProfile", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement(ResourceType.UserProfile, Operation.Update)));
});

// Register custom authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// In-memory data store for demonstration
var orders = new List<Order>
{
    new Order { Id = 1, OwnerId = "user-1", ProductName = "Laptop", Total = 999.99m, Status = "Delivered" },
    new Order { Id = 2, OwnerId = "user-2", ProductName = "Mouse", Total = 29.99m, Status = "Processing" },
    new Order { Id = 3, OwnerId = "user-1", ProductName = "Keyboard", Total = 79.99m, Status = "Shipped" }
};

var products = new List<Product>
{
    new Product { Id = 1, SellerId = "seller-1", Name = "Handmade Vase", Price = 45.00m, Category = "Home" },
    new Product { Id = 2, SellerId = "seller-2", Name = "Art Print", Price = 25.00m, Category = "Art" },
    new Product { Id = 3, SellerId = "seller-1", Name = "Custom Mug", Price = 15.00m, Category = "Home" }
};

var userProfiles = new List<UserProfile>
{
    new UserProfile { Id = 1, UserId = "user-1", Email = "user1@example.com", FullName = "John Doe" },
    new UserProfile { Id = 2, UserId = "user-2", Email = "user2@example.com", FullName = "Jane Smith" }
};

// GET /api/orders/{id} - Owner or Admin can view
app.MapGet("/api/orders/{id:int}", async (
    int id,
    IAuthorizationService authorizationService,
    HttpContext context) =>
{
    // Find the order
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound(new { Error = "Order not found" });
    }
    
    // Check authorization using the order as the resource
    var result = await authorizationService.AuthorizeAsync(
        context.User, 
        order, 
        "CanViewOrder");
    
    if (!result.Succeeded)
    {
        return Results.Forbid();
    }
    
    return Results.Ok(new
    {
        Message = "Order retrieved successfully",
        Order = order
    });
}).WithName("GetOrder").WithTags("Orders");

// PUT /api/orders/{id} - Only owner can update
app.MapPut("/api/orders/{id:int}", async (
    int id,
    UpdateOrderRequest request,
    IAuthorizationService authorizationService,
    HttpContext context) =>
{
    // Find the order
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound(new { Error = "Order not found" });
    }
    
    // Check authorization
    var result = await authorizationService.AuthorizeAsync(
        context.User, 
        order, 
        "CanUpdateOrder");
    
    if (!result.Succeeded)
    {
        return Results.Forbid();
    }
    
    // Update the order
    if (!string.IsNullOrEmpty(request.Status))
    {
        order.Status = request.Status;
    }
    
    return Results.Ok(new
    {
        Message = "Order updated successfully",
        Order = order
    });
}).WithName("UpdateOrder").WithTags("Orders");

// DELETE /api/orders/{id} - Owner or Admin can delete
app.MapDelete("/api/orders/{id:int}", async (
    int id,
    IAuthorizationService authorizationService,
    HttpContext context) =>
{
    // Find the order
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound(new { Error = "Order not found" });
    }
    
    // Check authorization
    var result = await authorizationService.AuthorizeAsync(
        context.User, 
        order, 
        "CanDeleteOrder");
    
    if (!result.Succeeded)
    {
        return Results.Forbid();
    }
    
    // Remove the order (in real app, soft delete would be better)
    orders.Remove(order);
    
    return Results.Ok(new
    {
        Message = "Order deleted successfully",
        OrderId = id
    });
}).WithName("DeleteOrder").WithTags("Orders");

// PUT /api/products/{id} - Only seller can update
app.MapPut("/api/products/{id:int}", async (
    int id,
    UpdateProductRequest request,
    IAuthorizationService authorizationService,
    HttpContext context) =>
{
    // Find the product
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
    {
        return Results.NotFound(new { Error = "Product not found" });
    }
    
    // Check authorization
    var result = await authorizationService.AuthorizeAsync(
        context.User, 
        product, 
        "CanUpdateProduct");
    
    if (!result.Succeeded)
    {
        return Results.Forbid();
    }
    
    // Update the product
    if (!string.IsNullOrEmpty(request.Name))
    {
        product.Name = request.Name;
    }
    if (request.Price.HasValue)
    {
        product.Price = request.Price.Value;
    }
    
    return Results.Ok(new
    {
        Message = "Product updated successfully",
        Product = product
    });
}).WithName("UpdateProduct").WithTags("Products");

// GET /api/users/{id}/profile - Only the user or Admin can view
app.MapGet("/api/users/{userId}/profile", async (
    string userId,
    IAuthorizationService authorizationService,
    HttpContext context) =>
{
    // Find the profile
    var profile = userProfiles.FirstOrDefault(p => p.UserId == userId);
    if (profile == null)
    {
        return Results.NotFound(new { Error = "Profile not found" });
    }
    
    // Check authorization
    var result = await authorizationService.AuthorizeAsync(
        context.User, 
        profile, 
        "CanViewProfile");
    
    if (!result.Succeeded)
    {
        return Results.Forbid();
    }
    
    return Results.Ok(new
    {
        Message = "Profile retrieved successfully",
        Profile = profile
    });
}).WithName("GetUserProfile").WithTags("Users");

// GET all orders (for the current user) - no resource auth needed, just authentication
app.MapGet("/api/orders", [Authorize] (HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var isAdmin = context.User.IsInRole("Admin");
    
    // Admin sees all orders, users see only their own
    var userOrders = isAdmin 
        ? orders 
        : orders.Where(o => o.OwnerId == userId).ToList();
    
    return Results.Ok(new
    {
        Message = isAdmin ? "All orders (Admin view)" : "Your orders",
        Orders = userOrders
    });
}).WithName("GetMyOrders").WithTags("Orders");

// Public endpoint
app.MapGet("/", () => "Resource-Based Authorization - ShopFlow API (Resource Auth Challenge)");

app.Run();

// ==================== RESOURCE MODELS ====================

public class Order
{
    public int Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Product
{
    public int Id { get; set; }
    public string SellerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class UserProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

// ==================== AUTHORIZATION REQUIREMENT ====================

public class ResourceOwnerRequirement : IAuthorizationRequirement
{
    public ResourceType ResourceType { get; }
    public Operation Operation { get; }
    
    public ResourceOwnerRequirement(ResourceType resourceType, Operation operation)
    {
        ResourceType = resourceType;
        Operation = operation;
    }
}

// ==================== AUTHORIZATION HANDLER ====================

public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement)
    {
        // Get current user ID
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Admin can do anything
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // Get the resource from the context
        var resource = context.Resource;
        
        if (resource == null)
        {
            return Task.CompletedTask;
        }
        
        // Check ownership based on resource type
        bool isOwner = false;
        
        switch (requirement.ResourceType)
        {
            case ResourceType.Order:
                if (resource is Order order)
                {
                    isOwner = order.OwnerId == userId;
                }
                break;
                
            case ResourceType.Product:
                if (resource is Product product)
                {
                    isOwner = product.SellerId == userId;
                }
                break;
                
            case ResourceType.UserProfile:
                if (resource is UserProfile profile)
                {
                    isOwner = profile.UserId == userId;
                }
                break;
        }
        
        // For Delete operations, also allow the owner
        // For Update operations, only the owner (already checked)
        // For Read operations, owner or Admin (Admin already checked above)
        
        if (isOwner)
        {
            // For Update: only owner can update
            // For Delete: owner or admin (admin already handled)
            // For Read: owner or admin (admin already handled)
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// ==================== ENUMS ====================

public enum ResourceType
{
    Order,
    Product,
    UserProfile
}

public enum Operation
{
    Read,
    Update,
    Delete
}

// ==================== SUPPORTING CLASSES ====================

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// Request/Response records
public record UpdateOrderRequest(string? Status);
public record UpdateProductRequest(string? Name, decimal? Price);

// Placeholder for ApplicationDbContext
public class ApplicationDbContext { }
