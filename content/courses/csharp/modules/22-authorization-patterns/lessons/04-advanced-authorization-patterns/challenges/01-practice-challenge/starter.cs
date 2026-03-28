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

// TODO: Configure Authorization with advanced patterns
// - Add policies for time-based access
// - Add policies for location-based access
// - Add policies for MFA requirements
// - Register all custom authorization handlers

// TODO: Register custom authorization handlers
// - AddSingleton<IAuthorizationHandler, BusinessHoursAuthorizationHandler>()
// - AddSingleton<IAuthorizationHandler, LocationAuthorizationHandler>()
// - AddSingleton<IAuthorizationHandler, MfaAuthorizationHandler>()
// - AddSingleton<IAuthorizationHandler, CompositeAuthorizationHandler>()

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// TODO: Create advanced authorization-protected endpoints
//
// 1. POST /api/admin/bulk-delete
//    - Requires BusinessHours (9-18) + Admin role + MFA
//    - Body: { "userIds": ["...", "..."] }
//    - Returns: deletion summary
//
// 2. GET /api/financial/reports
//    - Requires AllowedLocation (US, CA, UK only) + Admin role
//    - Query: ?reportType=monthly&month=12
//    - Returns: financial data
//
// 3. PUT /api/account/security-settings
//    - Requires MFA (within last 30 min) + Resource ownership
//    - Body: { "twoFactorEnabled": true, "recoveryEmail": "..." }
//    - Returns: updated settings
//
// 4. POST /api/admin/system-maintenance
//    - Requires Composite: BusinessHours + Location + MFA + Admin role
//    - Body: { "maintenanceType": "backup", "duration": 60 }
//    - Returns: maintenance scheduled confirmation
//
// 5. GET /api/admin/audit-logs
//    - Requires BusinessHours only
//    - Returns: system audit logs

// TODO: Implement custom authorization requirements
// 1. BusinessHoursRequirement
//    - Takes TimeSpan startTime, TimeSpan endTime
//    - Checks if current UTC time is within business hours
//    - Consider timezone (e.g., America/New_York)
//
// 2. LocationRequirement
//    - Takes string[] allowedCountries
//    - Checks 'Country' claim against allowed list
//    - Support wildcard '*' for any country
//
// 3. MfaRequirement
//    - Takes TimeSpan gracePeriod (e.g., 30 minutes)
//    - Checks 'MfaCompletedAt' claim timestamp
//    - Fails if MFA completed outside grace period
//
// 4. CompositeRequirement
//    - Takes multiple IAuthorizationRequirement[]
//    - Handler requires ALL to pass

// TODO: Implement authorization handlers
// - BusinessHoursAuthorizationHandler
// - LocationAuthorizationHandler  
// - MfaAuthorizationHandler
// - CompositeAuthorizationHandler

// Public endpoint
app.MapGet("/", () => "Advanced Authorization Patterns Challenge - ShopFlow API");

app.Run();

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? MfaCompletedAt { get; set; }
}

// TODO: Create request DTOs
public record BulkDeleteRequest(string[] UserIds);
public record SecuritySettingsRequest(bool TwoFactorEnabled, string? RecoveryEmail);
public record MaintenanceRequest(string MaintenanceType, int DurationMinutes);

// Placeholder for ApplicationDbContext
public class ApplicationDbContext { }
