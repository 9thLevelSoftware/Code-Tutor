using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Identity and Roles are configured
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register Claims Transformation service
builder.Services.AddScoped<IClaimsTransformation, PermissionClaimsTransformation>();

// Configure Authorization with Permission Policies
builder.Services.AddAuthorization(options =>
{
    // Permission-based policies
    options.AddPolicy("ViewReports", policy => 
        policy.RequireClaim("permission", Permissions.ViewReports));
    
    options.AddPolicy("EditProducts", policy => 
        policy.RequireClaim("permission", Permissions.EditProducts));
    
    options.AddPolicy("DeleteUsers", policy => 
        policy.RequireClaim("permission", Permissions.DeleteUsers));
    
    options.AddPolicy("ManageInventory", policy => 
        policy.RequireClaim("permission", Permissions.ManageInventory));
    
    options.AddPolicy("ProcessRefunds", policy => 
        policy.RequireClaim("permission", Permissions.ProcessRefunds));
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// ==================== PERMISSION-PROTECTED ENDPOINTS ====================

// GET /api/reports/sales - requires can:view-reports
app.MapGet("/api/reports/sales", [Authorize(Policy = "ViewReports")] () =>
{
    return Results.Ok(new
    {
        Report = "Sales Report",
        Period = "Last 30 Days",
        Data = new
        {
            TotalRevenue = 125000.00m,
            TotalOrders = 850,
            AverageOrderValue = 147.06m,
            TopProducts = new[]
            {
                new { Name = "Product A", Revenue = 45000 },
                new { Name = "Product B", Revenue = 32000 },
                new { Name = "Product C", Revenue = 28000 }
            }
        }
    });
}).WithName("GetSalesReport").WithTags("Reports");

// POST /api/products - requires can:edit-products
app.MapPost("/api/products", [Authorize(Policy = "EditProducts")] (CreateProductRequest request) =>
{
    var product = new
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Price = request.Price,
        Category = request.Category,
        CreatedAt = DateTime.UtcNow
    };
    
    return Results.Created($"/api/products/{product.Id}", product);
}).WithName("CreateProduct").WithTags("Products");

// DELETE /api/users/{id} - requires can:delete-users
app.MapDelete("/api/admin/users/{id}", [Authorize(Policy = "DeleteUsers")] (
    string id,
    HttpContext httpContext,
    IAuthorizationService authService) =>
{
    // Additional imperative check: prevent self-deletion
    var currentUserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (id == currentUserId)
    {
        return Results.BadRequest(new { Error = "Cannot delete your own account" });
    }
    
    // Simulate user deletion
    return Results.Ok(new { Message = $"User {id} deleted successfully" });
}).WithName("DeleteUser").WithTags("Admin");

// PUT /api/inventory/{id} - requires can:manage-inventory
app.MapPut("/api/inventory/{id}", [Authorize(Policy = "ManageInventory")] (
    int id,
    UpdateInventoryRequest request) =>
{
    return Results.Ok(new
    {
        ProductId = id,
        NewQuantity = request.Quantity,
        UpdatedAt = DateTime.UtcNow,
        Message = "Inventory updated successfully"
    });
}).WithName("UpdateInventory").WithTags("Inventory");

// POST /api/orders/{id}/refund - uses imperative authorization
app.MapPost("/api/orders/{id}/refund", async (
    int id,
    RefundRequest request,
    IAuthorizationService authorizationService,
    HttpContext httpContext) =>
{
    // Imperative authorization check
    var result = await authorizationService.AuthorizeAsync(
        httpContext.User, 
        null, 
        "ProcessRefunds");
    
    if (!result.Succeeded)
    {
        return Results.Forbid();
    }
    
    return Results.Ok(new
    {
        OrderId = id,
        RefundAmount = request.Amount,
        Reason = request.Reason,
        ProcessedAt = DateTime.UtcNow,
        RefundId = Guid.NewGuid()
    });
}).WithName("ProcessRefund").WithTags("Orders");

// GET /api/my-permissions - return current user's permissions
app.MapGet("/api/my-permissions", [Authorize] (HttpContext httpContext) =>
{
    var permissions = httpContext.User
        .FindAll("permission")
        .Select(c => c.Value)
        .ToList();
    
    return Results.Ok(new
    {
        UserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
        Email = httpContext.User.FindFirstValue(ClaimTypes.Email),
        Permissions = permissions,
        Roles = httpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
    });
}).WithName("GetMyPermissions").WithTags("User");

app.MapGet("/", () => "Claims-Based Authorization - ShopFlow API");

app.Run();

// ==================== PERMISSIONS CONSTANTS ====================

public static class Permissions
{
    public const string ViewReports = "can:view-reports";
    public const string EditProducts = "can:edit-products";
    public const string DeleteUsers = "can:delete-users";
    public const string ManageInventory = "can:manage-inventory";
    public const string ProcessRefunds = "can:process-refunds";
    
    public static IReadOnlyList<string> All => new[]
    {
        ViewReports,
        EditProducts,
        DeleteUsers,
        ManageInventory,
        ProcessRefunds
    };
}

// ==================== CLAIMS TRANSFORMATION ====================

public class PermissionClaimsTransformation : IClaimsTransformation
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PermissionClaimsTransformation> _logger;

    public PermissionClaimsTransformation(
        UserManager<ApplicationUser> userManager,
        ILogger<PermissionClaimsTransformation> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Check if already transformed (avoid infinite loops and redundant work)
        if (principal.HasClaim(c => c.Type == "permission"))
        {
            return principal;
        }

        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
        {
            return principal;
        }

        // Get user ID from existing claims
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return principal;
        }

        // Find user and their roles
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return principal;
        }

        var roles = await _userManager.GetRolesAsync(user);
        
        // Map roles to permission claims
        var permissions = MapRolesToPermissions(roles);
        
        foreach (var permission in permissions)
        {
            identity.AddClaim(new Claim("permission", permission));
            _logger.LogDebug("Added permission claim: {Permission} for user {UserId}", 
                permission, userId);
        }

        return principal;
    }

    private static IEnumerable<string> MapRolesToPermissions(IList<string> roles)
    {
        var permissions = new HashSet<string>();

        foreach (var role in roles)
        {
            switch (role)
            {
                case "Admin":
                    // Admin gets all permissions
                    foreach (var perm in Permissions.All)
                    {
                        permissions.Add(perm);
                    }
                    break;
                    
                case "Manager":
                    permissions.Add(Permissions.ViewReports);
                    permissions.Add(Permissions.ManageInventory);
                    break;
                    
                case "User":
                    permissions.Add(Permissions.EditProducts);
                    break;
                    
                case "SupportService":
                    permissions.Add(Permissions.ProcessRefunds);
                    break;
            }
        }

        return permissions;
    }
}

// ==================== ENTITY CLASSES ====================

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// ==================== REQUEST RECORDS ====================

public record CreateProductRequest(string Name, decimal Price, string Category);
public record UpdateInventoryRequest(int Quantity);
public record RefundRequest(decimal Amount, string Reason);

// ==================== DB CONTEXT ====================

public class ApplicationDbContext { }
