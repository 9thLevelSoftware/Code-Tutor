using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// TODO: Configure Identity with Role support
// - Add AddIdentity<ApplicationUser, IdentityRole>()
// - Include AddRoles<IdentityRole>()
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
    // - Ensure Admin, User, Guest roles exist
    // - Create default admin user
}

// TODO: Create role-protected endpoints
// 
// 1. GET /api/admin/dashboard
//    - Only accessible to "Admin" role
//    - Returns: { "message": "Admin dashboard data", "stats": {...} }
//
// 2. GET /api/user/profile
//    - Accessible to "User" and "Admin" roles  
//    - Returns: { "userId": "...", "email": "...", "role": "..." }
//
// 3. GET /api/public/catalog
//    - Accessible to all authenticated users (any role)
//    - Returns: { "products": [...] }
//
// 4. POST /api/admin/users/assign-role
//    - Only "Admin" can assign roles
//    - Body: { "userId": "...", "role": "..." }
//    - Returns: success/failure result

app.MapGet("/", () => "Role-Based Access Control Challenge - ShopFlow API");

app.Run();

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// TODO: Create a RoleSeeder class
// - Inject RoleManager<IdentityRole> and UserManager<ApplicationUser>
// - Create SeedAsync method that ensures roles exist
// - Create default admin user: admin@shopflow.com / Admin123!
