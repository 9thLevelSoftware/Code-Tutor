using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Identity and Roles are configured
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// TODO: Register Claims Transformation service
// builder.Services.AddScoped<IClaimsTransformation, PermissionClaimsTransformation>();

// TODO: Configure Authorization with Permission Policies
builder.Services.AddAuthorization(options =>
{
    // TODO: Add policies for each permission:
    // - ViewReports: requires claim "permission" with value "can:view-reports"
    // - EditProducts: requires claim "permission" with value "can:edit-products"
    // - DeleteUsers: requires claim "permission" with value "can:delete-users"
    // - ManageInventory: requires claim "permission" with value "can:manage-inventory"
    // - ProcessRefunds: requires claim "permission" with value "can:process-refunds"
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// TODO: Create permission-protected endpoints
//
// 1. GET /api/reports/sales
//    - Requires: can:view-reports permission
//    - Returns: sales report data
//
// 2. POST /api/products
//    - Requires: can:edit-products permission
//    - Body: product data
//
// 3. DELETE /api/users/{id}
//    - Requires: can:delete-users permission
//    - Also check: prevent deleting own account
//
// 4. PUT /api/inventory/{id}
//    - Requires: can:manage-inventory permission
//    - Updates inventory levels
//
// 5. POST /api/orders/{id}/refund
//    - Use IAuthorizationService.AuthorizeAsync for imperative check
//    - Requires: can:process-refunds permission
//
// 6. GET /api/my-permissions
//    - Returns: array of current user's permission claims
//    - No special authorization required (authenticated users)

app.MapGet("/", () => "Claims-Based Authorization Challenge - ShopFlow API");

app.Run();

// TODO: Create PermissionClaimsTransformation class
// - Inject UserManager<ApplicationUser> and RoleManager<IdentityRole>
// - In TransformAsync, check user's roles and add permission claims
// - Mapping: Admin→all, Manager→view-reports+manage-inventory, etc.

// TODO: Create Permissions constants class
// public static class Permissions
// {
//     public const string ViewReports = "can:view-reports";
//     ...
// }

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class ApplicationDbContext { }
