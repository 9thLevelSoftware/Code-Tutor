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

// TODO: Configure Authorization with custom policies
// 1. Add policy 'Age18Plus' that requires 'Age' claim >= 18
// 2. Add policy 'CanEditProducts' that requires 'Permission' claim containing 'CanEditProducts'
// 3. Add policy 'AdminAndAdult' that requires Admin role AND Age >= 18
// 4. Add policy 'PremiumCustomer' that requires 'IsPremium' claim = 'true'
// 5. Add policy 'CanDeleteProducts' that requires 'CanDeleteProducts' permission

// TODO: Register custom authorization handlers
// - AddSingleton<IAuthorizationHandler, AgeAuthorizationHandler>()
// - AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>()
// - AddSingleton<IAuthorizationHandler, PremiumCustomerHandler>()

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// TODO: Create policy-protected endpoints
//
// 1. GET /api/products/restricted
//    - Requires 'Age18Plus' policy (mature content)
//    - Returns: { "message": "Age-restricted products", "products": [...] }
//
// 2. POST /api/products
//    - Requires 'CanEditProducts' policy
//    - Body: { "name": "...", "price": ..., "category": "..." }
//    - Returns: created product with 201 status
//
// 3. GET /api/admin/advanced
//    - Requires 'AdminAndAdult' policy (Admin + 18+)
//    - Returns: { "message": "Advanced admin panel", "sensitiveData": {...} }
//
// 4. GET /api/customer/premium-content
//    - Requires 'PremiumCustomer' policy
//    - Returns: { "message": "Premium content", "discounts": [...], "features": [...] }
//
// 5. DELETE /api/products/{id}
//    - Requires 'CanDeleteProducts' policy
//    - Returns: { "message": "Product deleted", "productId": ... }

// Public endpoint
app.MapGet("/api/public/products", () =>
{
    return Results.Ok(new
    {
        Message = "Public product catalog",
        Products = new[]
        {
            new { Id = 1, Name = "T-Shirt", Price = 29.99m },
            new { Id = 2, Name = "Coffee Mug", Price = 15.99m }
        }
    });
});

app.MapGet("/", () => "Policy-Based Authorization Challenge - ShopFlow API");

app.Run();

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? Age { get; set; }
    public bool IsPremium { get; set; }
}

// TODO: Create custom authorization requirements
// 1. AgeRequirement - takes minimum age as parameter
// 2. PermissionRequirement - takes permission name as parameter
// 3. PremiumCustomerRequirement - no parameters needed

// TODO: Create authorization handlers
// 1. AgeAuthorizationHandler : AuthorizationHandler<AgeRequirement>
//    - Check context.User.HasClaim(c => c.Type == "Age")
//    - Parse age claim and compare against requirement.MinimumAge
//    - Call context.Succeed(requirement) if age >= minimum
//
// 2. PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
//    - Check for 'Permission' claim with the required permission value
//    - Support multiple permissions separated by comma in claim
//
// 3. PremiumCustomerHandler : AuthorizationHandler<PremiumCustomerRequirement>
//    - Check for 'IsPremium' claim with value 'true'
//    - OR check if user has 'Premium' role

// TODO: Create DTOs
public record CreateProductRequest(string Name, decimal Price, string Category);
