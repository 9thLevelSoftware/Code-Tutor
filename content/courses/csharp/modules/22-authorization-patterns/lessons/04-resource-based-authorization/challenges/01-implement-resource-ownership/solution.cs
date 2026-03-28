using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Identity is configured
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// Register Authorization Handlers
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ProductAuthorizationHandler>();

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

// ==================== DOCUMENT ENDPOINTS ====================

// GET /api/documents/{id} - Allow if owner OR document is public
app.MapGet("/api/documents/{id:int}", async (
    int id,
    IAuthorizationService authorizationService,
    HttpContext httpContext) =>
{
    var document = documents.FirstOrDefault(d => d.Id == id);
    if (document == null)
        return Results.NotFound(new { Error = "Document not found" });

    // Check if owner OR public
    var authResult = await authorizationService.AuthorizeAsync(
        httpContext.User, document, Operations.Read);

    if (!authResult.Succeeded)
        return Results.Forbid();

    return Results.Ok(document);
}).WithName("GetDocument").WithTags("Documents");

// PUT /api/documents/{id} - Only owner can edit
app.MapPut("/api/documents/{id:int}", async (
    int id,
    UpdateDocumentRequest request,
    IAuthorizationService authorizationService,
    HttpContext httpContext) =>
{
    var document = documents.FirstOrDefault(d => d.Id == id);
    if (document == null)
        return Results.NotFound(new { Error = "Document not found" });

    // Check ownership for Update
    var authResult = await authorizationService.AuthorizeAsync(
        httpContext.User, document, Operations.Update);

    if (!authResult.Succeeded)
        return Results.Forbid();

    // Update document
    document.Title = request.Title ?? document.Title;
    document.Content = request.Content ?? document.Content;
    document.IsPublic = request.IsPublic ?? document.IsPublic;

    return Results.Ok(new
    {
        Message = "Document updated successfully",
        Document = document
    });
}).WithName("UpdateDocument").WithTags("Documents");

// DELETE /api/documents/{id} - Only owner can delete
app.MapDelete("/api/documents/{id:int}", async (
    int id,
    IAuthorizationService authorizationService,
    HttpContext httpContext) =>
{
    var document = documents.FirstOrDefault(d => d.Id == id);
    if (document == null)
        return Results.NotFound(new { Error = "Document not found" });

    // Check ownership for Delete
    var authResult = await authorizationService.AuthorizeAsync(
        httpContext.User, document, Operations.Delete);

    if (!authResult.Succeeded)
        return Results.Forbid();

    documents.Remove(document);

    return Results.Ok(new { Message = "Document deleted successfully" });
}).WithName("DeleteDocument").WithTags("Documents");

// ==================== PRODUCT ENDPOINTS ====================

// GET /api/products/{id} - Anyone authenticated can view
app.MapGet("/api/products/{id:int}", [Authorize] (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
        return Results.NotFound(new { Error = "Product not found" });

    return Results.Ok(product);
}).WithName("GetProduct").WithTags("Products");

// PUT /api/products/{id} - Only owner can edit
app.MapPut("/api/products/{id:int}", async (
    int id,
    UpdateProductRequest request,
    IAuthorizationService authorizationService,
    HttpContext httpContext) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
        return Results.NotFound(new { Error = "Product not found" });

    // Check ownership for Update
    var authResult = await authorizationService.AuthorizeAsync(
        httpContext.User, product, Operations.Update);

    if (!authResult.Succeeded)
        return Results.Forbid();

    // Update product
    product.Name = request.Name ?? product.Name;
    product.Price = request.Price ?? product.Price;
    product.Status = request.Status ?? product.Status;

    return Results.Ok(new
    {
        Message = "Product updated successfully",
        Product = product
    });
}).WithName("UpdateProduct").WithTags("Products");

// DELETE /api/products/{id} - Only owner can delete
app.MapDelete("/api/products/{id:int}", async (
    int id,
    IAuthorizationService authorizationService,
    HttpContext httpContext) =>
{
    var product = products.FirstOr(p => p.Id == id);
    if (product == null)
        return Results.NotFound(new { Error = "Product not found" });

    // Check ownership for Delete
    var authResult = await authorizationService.AuthorizeAsync(
        httpContext.User, product, Operations.Delete);

    if (!authResult.Succeeded)
        return Results.Forbid();

    products.Remove(product);

    return Results.Ok(new { Message = "Product deleted successfully" });
}).WithName("DeleteProduct").WithTags("Products");

// POST /api/products - Create new (any authenticated user)
app.MapPost("/api/products", [Authorize] (
    CreateProductRequest request,
    HttpContext httpContext) =>
{
    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    var product = new Product
    {
        Id = products.Max(p => p.Id) + 1,
        Name = request.Name,
        Price = request.Price,
        OwnerId = userId!,
        Status = "Active",
        CreatedAt = DateTime.UtcNow
    };

    products.Add(product);

    return Results.Created($"/api/products/{product.Id}", product);
}).WithName("CreateProduct").WithTags("Products");

app.MapGet("/", () => "Resource-Based Authorization - ShopFlow API");

app.Run();

// ==================== AUTHORIZATION HANDLERS ====================

public class DocumentAuthorizationHandler : 
    AuthorizationHandler<OperationAuthorizationRequirement, Document>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Document resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = context.User.IsInRole("Admin");

        // Admin can do anything
        if (isAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        switch (requirement.Name)
        {
            case nameof(Operations.Read):
                // Owner can read, or anyone if public
                if (resource.OwnerId == userId || resource.IsPublic)
                {
                    context.Succeed(requirement);
                }
                break;

            case nameof(Operations.Update):
            case nameof(Operations.Delete):
                // Only owner can update/delete
                if (resource.OwnerId == userId)
                {
                    context.Succeed(requirement);
                }
                break;
        }

        return Task.CompletedTask;
    }
}

public class ProductAuthorizationHandler : 
    AuthorizationHandler<OperationAuthorizationRequirement, Product>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Product resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = context.User.IsInRole("Admin");

        // Admin can do anything
        if (isAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        switch (requirement.Name)
        {
            case nameof(Operations.Update):
            case nameof(Operations.Delete):
                // Only owner can update/delete
                if (resource.OwnerId == userId)
                {
                    context.Succeed(requirement);
                }
                break;
        }

        return Task.CompletedTask;
    }
}

// ==================== OPERATIONS ====================

public static class Operations
{
    public static OperationAuthorizationRequirement Create =
        new() { Name = nameof(Create) };
    public static OperationAuthorizationRequirement Read =
        new() { Name = nameof(Read) };
    public static OperationAuthorizationRequirement Update =
        new() { Name = nameof(Update) };
    public static OperationAuthorizationRequirement Delete =
        new() { Name = nameof(Delete) };
}

// ==================== ENTITY CLASSES ====================

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

// ==================== REQUEST RECORDS ====================

public record UpdateDocumentRequest(string? Title, string? Content, bool? IsPublic);
public record UpdateProductRequest(string? Name, decimal? Price, string? Status);
public record CreateProductRequest(string Name, decimal Price);

// ==================== DB CONTEXT ====================

public class ApplicationDbContext { }
