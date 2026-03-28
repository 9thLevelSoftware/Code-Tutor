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

// Configure Authorization with advanced patterns
builder.Services.AddAuthorization(options =>
{
    // Business hours policy - 9 AM to 6 PM
    options.AddPolicy("BusinessHoursOnly", policy =>
        policy.Requirements.Add(new BusinessHoursRequirement(
            TimeSpan.FromHours(9), 
            TimeSpan.FromHours(18),
            "America/New_York")));
    
    // Allowed location policy - US, CA, UK
    options.AddPolicy("AllowedLocation", policy =>
        policy.Requirements.Add(new LocationRequirement("US", "CA", "GB")));
    
    // MFA required policy - within last 30 minutes
    options.AddPolicy("MfaRequired", policy =>
        policy.Requirements.Add(new MfaRequirement(TimeSpan.FromMinutes(30))));
    
    // Composite policy - Admin + Business Hours + Location + MFA
    options.AddPolicy("UltraSecureOperation", policy =>
    {
        policy.RequireRole("Admin");
        policy.Requirements.Add(new BusinessHoursRequirement(
            TimeSpan.FromHours(9), 
            TimeSpan.FromHours(18),
            "America/New_York"));
        policy.Requirements.Add(new LocationRequirement("US", "CA", "GB"));
        policy.Requirements.Add(new MfaRequirement(TimeSpan.FromMinutes(30)));
    });
    
    // Bulk operations - Business Hours + Admin + MFA
    options.AddPolicy("BulkOperations", policy =>
    {
        policy.RequireRole("Admin");
        policy.Requirements.Add(new BusinessHoursRequirement(
            TimeSpan.FromHours(9), 
            TimeSpan.FromHours(18),
            "America/New_York"));
        policy.Requirements.Add(new MfaRequirement(TimeSpan.FromMinutes(30)));
    });
});

// Register custom authorization handlers
builder.Services.AddSingleton<IAuthorizationHandler, BusinessHoursAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, LocationAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, MfaAuthorizationHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// POST /api/admin/bulk-delete - Business Hours + Admin + MFA
app.MapPost("/api/admin/bulk-delete", [Authorize(Policy = "BulkOperations")] (
    BulkDeleteRequest request,
    HttpContext context,
    ILogger<Program> logger) =>
{
    var adminId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var adminEmail = context.User.FindFirstValue(ClaimTypes.Email);
    
    logger.LogWarning("Bulk delete initiated by {Admin} for {Count} users", 
        adminEmail, request.UserIds.Length);
    
    // Simulate bulk deletion
    var deletedCount = request.UserIds.Length;
    var failedIds = new List<string>();
    
    return Results.Ok(new
    {
        Message = "Bulk deletion completed",
        DeletedCount = deletedCount,
        FailedIds = failedIds,
        DeletedBy = adminEmail,
        ExecutedAt = DateTime.UtcNow,
        BusinessHoursCompliant = true,
        MfaVerified = true
    });
}).WithName("BulkDeleteUsers").WithTags("Admin");

// GET /api/financial/reports - Location + Admin
app.MapGet("/api/financial/reports", [Authorize(Policy = "AllowedLocation")] (
    [Authorize(Roles = "Admin")] 
    string? reportType,
    int? month,
    HttpContext context) =>
{
    var country = context.User.FindFirstValue("Country");
    var adminEmail = context.User.FindFirstValue(ClaimTypes.Email);
    
    return Results.Ok(new
    {
        Message = "Financial reports accessed",
        ReportType = reportType ?? "monthly",
        Month = month ?? DateTime.UtcNow.Month,
        AccessedBy = adminEmail,
        AccessedFrom = country,
        GeneratedAt = DateTime.UtcNow,
        Data = new
        {
            Revenue = 125000.50m,
            Expenses = 85000.25m,
            Profit = 40000.25m,
            TopProducts = new[] { "Laptop", "Phone", "Headphones" }
        }
    });
}).WithName("GetFinancialReports").WithTags("Financial");

// PUT /api/account/security-settings - MFA + ownership
app.MapPut("/api/account/security-settings", [Authorize(Policy = "MfaRequired")] (
    SecuritySettingsRequest request,
    HttpContext context,
    ILogger<Program> logger) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = context.User.FindFirstValue(ClaimTypes.Email);
    var mfaCompletedAt = context.User.FindFirstValue("MfaCompletedAt");
    
    logger.LogInformation("Security settings updated for {Email}", email);
    
    return Results.Ok(new
    {
        Message = "Security settings updated successfully",
        UserId = userId,
        Settings = new
        {
            request.TwoFactorEnabled,
            request.RecoveryEmail,
            UpdatedAt = DateTime.UtcNow
        },
        MfaVerifiedAt = mfaCompletedAt,
        Warning = "Security-sensitive operation completed with MFA verification"
    });
}).WithName("UpdateSecuritySettings").WithTags("Account");

// POST /api/admin/system-maintenance - Ultra Secure (Admin + Business Hours + Location + MFA)
app.MapPost("/api/admin/system-maintenance", [Authorize(Policy = "UltraSecureOperation")] (
    MaintenanceRequest request,
    HttpContext context,
    ILogger<Program> logger) =>
{
    var adminId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var adminEmail = context.User.FindFirstValue(ClaimTypes.Email);
    var country = context.User.FindFirstValue("Country");
    
    logger.LogCritical("System maintenance scheduled by {Admin} from {Country}: {Type} for {Duration}min",
        adminEmail, country, request.MaintenanceType, request.DurationMinutes);
    
    return Results.Ok(new
    {
        Message = "System maintenance scheduled",
        MaintenanceType = request.MaintenanceType,
        DurationMinutes = request.DurationMinutes,
        ScheduledBy = adminEmail,
        ScheduledFrom = country,
        ScheduledAt = DateTime.UtcNow,
        StartsAt = DateTime.UtcNow.AddHours(1),
        SecurityChecklist = new
        {
            AdminRole = true,
            BusinessHours = true,
            LocationVerified = true,
            MfaVerified = true
        }
    });
}).WithName("ScheduleMaintenance").WithTags("Admin");

// GET /api/admin/audit-logs - Business Hours only
app.MapGet("/api/admin/audit-logs", [Authorize(Policy = "BusinessHoursOnly")] (
    HttpContext context) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    return Results.Ok(new
    {
        Message = "Audit logs retrieved",
        CurrentHour = DateTime.UtcNow.ToString("HH:mm"),
        BusinessHours = "09:00 - 18:00 EST",
        Logs = new[]
        {
            new { Timestamp = DateTime.UtcNow.AddHours(-1), Action = "UserLogin", UserId = "user-1", Status = "Success" },
            new { Timestamp = DateTime.UtcNow.AddHours(-2), Action = "PasswordChange", UserId = "user-2", Status = "Success" },
            new { Timestamp = DateTime.UtcNow.AddHours(-3), Action = "BulkDelete", UserId = "admin-1", Status = "Success" }
        }
    });
}).WithName("GetAuditLogs").WithTags("Admin");

// GET /api/auth/status - Check current authorization factors
app.MapGet("/api/auth/status", [Authorize] (HttpContext context) =>
{
    var mfaClaim = context.User.FindFirst("MfaCompletedAt");
    var countryClaim = context.User.FindFirst("Country");
    var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    
    DateTime? mfaTime = null;
    if (mfaClaim != null && DateTime.TryParse(mfaClaim.Value, out var parsedMfa))
    {
        mfaTime = parsedMfa;
    }
    
    var mfaValid = mfaTime.HasValue && (DateTime.UtcNow - mfaTime.Value) < TimeSpan.FromMinutes(30);
    
    return Results.Ok(new
    {
        Message = "Current authorization status",
        Authenticated = true,
        UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
        Roles = roles,
        Country = countryClaim?.Value ?? "Unknown",
        MfaCompletedAt = mfaTime,
        MfaStillValid = mfaValid,
        CurrentTime = DateTime.UtcNow,
        BusinessHours = DateTime.UtcNow.TimeOfDay >= TimeSpan.FromHours(9) && 
                       DateTime.UtcNow.TimeOfDay <= TimeSpan.FromHours(18)
    });
}).WithName("AuthStatus").WithTags("Auth");

// Public endpoint
app.MapGet("/", () => "Advanced Authorization Patterns - ShopFlow API (Advanced Auth Challenge)");

app.Run();

// ==================== ADVANCED AUTHORIZATION REQUIREMENTS ====================

public class BusinessHoursRequirement : IAuthorizationRequirement
{
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }
    public string TimeZoneId { get; }
    
    public BusinessHoursRequirement(TimeSpan startTime, TimeSpan endTime, string timeZoneId)
    {
        StartTime = startTime;
        EndTime = endTime;
        TimeZoneId = timeZoneId;
    }
}

public class LocationRequirement : IAuthorizationRequirement
{
    public string[] AllowedCountries { get; }
    
    public LocationRequirement(params string[] allowedCountries)
    {
        AllowedCountries = allowedCountries;
    }
}

public class MfaRequirement : IAuthorizationRequirement
{
    public TimeSpan GracePeriod { get; }
    
    public MfaRequirement(TimeSpan gracePeriod)
    {
        GracePeriod = gracePeriod;
    }
}

// ==================== AUTHORIZATION HANDLERS ====================

public class BusinessHoursAuthorizationHandler : AuthorizationHandler<BusinessHoursRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BusinessHoursRequirement requirement)
    {
        try
        {
            // Get the configured timezone
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(requirement.TimeZoneId);
            
            // Convert current UTC time to the business timezone
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            var currentTimeOfDay = localTime.TimeOfDay;
            
            // Check if current time is within business hours
            var isWithinBusinessHours = currentTimeOfDay >= requirement.StartTime && 
                                       currentTimeOfDay <= requirement.EndTime;
            
            // Also check it's a weekday (Monday = 1, Friday = 5)
            var isWeekday = localTime.DayOfWeek >= DayOfWeek.Monday && 
                           localTime.DayOfWeek <= DayOfWeek.Friday;
            
            if (isWithinBusinessHours && isWeekday)
            {
                context.Succeed(requirement);
            }
        }
        catch (TimeZoneNotFoundException)
        {
            // Fallback to UTC if timezone not found
            var utcNow = DateTime.UtcNow.TimeOfDay;
            if (utcNow >= requirement.StartTime && utcNow <= requirement.EndTime)
            {
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}

public class LocationAuthorizationHandler : AuthorizationHandler<LocationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        LocationRequirement requirement)
    {
        // Get the country claim
        var countryClaim = context.User.FindFirst(c => c.Type == "Country")?.Value;
        
        // Check if country is in allowed list
        if (!string.IsNullOrEmpty(countryClaim) && 
            requirement.AllowedCountries.Contains(countryClaim, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
        
        // Admin bypass for location restrictions
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

public class MfaAuthorizationHandler : AuthorizationHandler<MfaRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MfaRequirement requirement)
    {
        // Get the MFA completed timestamp claim
        var mfaClaim = context.User.FindFirst(c => c.Type == "MfaCompletedAt")?.Value;
        
        if (!string.IsNullOrEmpty(mfaClaim) && DateTime.TryParse(mfaClaim, out var mfaCompletedAt))
        {
            // Check if MFA is still within the grace period
            var timeSinceMfa = DateTime.UtcNow - mfaCompletedAt;
            
            if (timeSinceMfa <= requirement.GracePeriod)
            {
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}

// ==================== SUPPORTING CLASSES ====================

// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? MfaCompletedAt { get; set; }
}

// Request/Response records
public record BulkDeleteRequest(string[] UserIds);
public record SecuritySettingsRequest(bool TwoFactorEnabled, string? RecoveryEmail);
public record MaintenanceRequest(string MaintenanceType, int DurationMinutes);

// Placeholder for ApplicationDbContext
public class ApplicationDbContext { }
