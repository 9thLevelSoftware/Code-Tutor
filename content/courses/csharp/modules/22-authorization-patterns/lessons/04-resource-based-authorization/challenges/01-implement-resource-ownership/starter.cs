using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Identity is configured
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// TODO: Register Authorization Handlers
// builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
// builder.Services.AddSingleton<IAuthorizationHandler, ProductAuthorizationHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Simulated data store
var documents = new List<Document>
{
    new() { Id = 1, Title = "My Document", Content = "Content", OwnerId = "user1", IsPublic = false },
    new() { Id = 2, Title = "Public Doc", Content = "Public", OwnerId = "user2", IsPublic = true }
};

var products = new List<Product>
{
    new() { Id = 1, Name = "Product A", Price = 99.99m, OwnerId = "user1", Status = "Active" },
    new() { Id = 2, Name = "Product B", Price = 49.99m, OwnerId = "user2", Status = "Active" }
};

// TODO: Create Document endpoints with resource-based authorization
//
// GET /api/documents/{id}
// - Load document
// - Return 404 if not found
// - Check auth: owner OR public
// - Return document if authorized
//
// PUT /api/documents/{id}
// - Load document
// - Use IAuthorizationService to check Update permission
// - Only owner (or Admin) can update
//
// DELETE /api/documents/{id}
// - Load document
// - Use IAuthorizationService to check Delete permission
// - Only owner (or Admin) can delete

// TODO: Create Product endpoints with resource-based authorization
//
// GET /api/products/{id}
// - Anyone can view (no resource auth needed, just authentication)
//
// PUT /api/products/{id}
// - Only owner can edit
// - Use resource-based authorization
//
// DELETE /api/products/{id}
// - Only owner can delete
// - Use resource-based authorization

app.MapGet("/", () => "Resource-Based Authorization Challenge - ShopFlow API");

app.Run();

// TODO: Create DocumentAuthorizationHandler
// - Inherit from AuthorizationHandler<OperationAuthorizationRequirement, Document>
// - Check if context.User is owner OR Admin
// - Call Succeed if authorized

// TODO: Create ProductAuthorizationHandler  
// - Inherit from AuthorizationHandler<OperationAuthorizationRequirement, Product>
// - Check if context.User is owner OR Admin
// - Call Succeed if authorized

// Entity classes
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// TODO: Create static Operations class
// public static class Operations
// {
//     public static OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
//     ...
// }

public class ApplicationDbContext { }
