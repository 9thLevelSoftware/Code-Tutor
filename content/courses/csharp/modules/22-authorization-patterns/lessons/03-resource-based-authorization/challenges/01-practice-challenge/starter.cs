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

// TODO: Configure Authorization
// - Add policies for resource-based authorization
// - Register ResourceOwnerAuthorizationHandler

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

// TODO: Create resource-protected endpoints
//
// 1. GET /api/orders/{id}
//    - Load order by id
//    - Check if current user owns the order OR is Admin
//    - Return order details or 403 Forbidden
//
// 2. PUT /api/orders/{id}
//    - Load order by id
//    - Only owner can update their order
//    - Body: { "status": "..." }
//    - Return updated order or 403
//
// 3. DELETE /api/orders/{id}
//    - Load order by id
//    - Owner or Admin can delete
//    - Return success or 403
//
// 4. PUT /api/products/{id}
//    - Load product by id
//    - Only seller (product owner) can update
//    - Body: { "price": ..., "name": "..." }
//    - Return updated product or 403
//
// 5. GET /api/users/{id}/profile
//    - Load profile by user id
//    - Only the user or Admin can view
//    - Return profile or 403

// TODO: Implement ResourceOwnerAuthorizationHandler
// - Should handle ResourceOwnerRequirement
// - Check if context.Resource is Order/Product/UserProfile
// - Compare resource.OwnerId/SellerId/UserId with current user id
// - Always allow Admin to bypass ownership check

// Public endpoint
app.MapGet("/", () => "Resource-Based Authorization Challenge - ShopFlow API");

app.Run();

// Resource models with ownership
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

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// TODO: Create ResourceOwnerRequirement
// - Should take ResourceType enum and Operation enum as parameters
// - ResourceType: Order, Product, UserProfile
// - Operation: Read, Update, Delete

// TODO: Create ResourceType and Operation enums

// Placeholder for ApplicationDbContext
public class ApplicationDbContext { }
