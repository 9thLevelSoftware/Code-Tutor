using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// TODO: Configure Identity with Role support
// - Add AddIdentity<ApplicationUser, IdentityRole>()
// - Include AddRoles<IdentityRole>() to enable role management
// - AddEntityFrameworkStores (assume ApplicationDbContext is configured)

// JWT Authentication is already configured
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

// TODO: Add Authorization services
// - Call AddAuthorization()

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Seed roles on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // TODO: Implement role seeder
    // - Ensure Admin, Manager, Customer roles exist
    // - Create default admin user
    // - Create test manager user
}

// TODO: Create role-protected endpoints
// 
// 1. GET /api/admin/dashboard
//    - Only accessible to "Admin" role
//    - Returns: { "message": "Admin dashboard", "stats": {...}, "totalRevenue": ... }
//
// 2. GET /api/manager/inventory
//    - Accessible to "Admin" and "Manager" roles
//    - Returns: { "products": [...], "lowStockAlerts": [...] }
//
// 3. GET /api/customer/orders
//    - Accessible to "Customer" and "Admin" roles
//    - Returns: { "orders": [...], "customerId": ... }
//
// 4. POST /api/admin/users/promote
//    - Only "Admin" can promote users to Manager
//    - Body: { "userId": "...", "newRole": "Manager" }
//    - Returns: success/failure result with role assignment confirmation
//
// 5. DELETE /api/admin/products/{id}
//    - Only "Admin" can delete products
//    - Returns: { "message": "Product deleted", "productId": ... }

app.MapGet("/", () => "Role-Based Access Control Challenge - ShopFlow API");

app.Run();

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// TODO: Create a RoleSeeder class
// - Inject RoleManager<IdentityRole> and UserManager<ApplicationUser>
// - Create SeedAsync method that ensures roles exist
// - Create default admin user: admin@shopflow.com / Admin123!
// - Create test manager user: manager@shopflow.com / Manager123!

// TODO: Create DTOs for role assignment
public record PromoteUserRequest(string UserId, string NewRole);
