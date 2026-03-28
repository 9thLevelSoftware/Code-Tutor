using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Services are already configured
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// TODO: Add HttpContextAccessor for accessing current user in services
// builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// TODO: Create Admin Dashboard endpoints
// 
// 1. User Management:
//    GET /api/admin/users - List all users with pagination
//    GET /api/admin/users/{id} - Get user details with roles
//    PUT /api/admin/users/{id}/status - Enable/disable user
//    DELETE /api/admin/users/{id} - Soft-delete user
//
// 2. Role Management:
//    GET /api/admin/roles - List all roles
//    POST /api/admin/users/{id}/roles - Add role to user
//    DELETE /api/admin/users/{id}/roles/{role} - Remove role
//    GET /api/admin/users/{id}/permissions - Get user permissions
//
// 3. Audit Logging:
//    GET /api/admin/audit-logs - View audit trail
//    - Create AuditLog entity
//    - Log all admin actions
//
// 4. Dashboard Stats:
//    GET /api/admin/stats - Return user statistics

app.MapGet("/", () => "Admin Dashboard Challenge - ShopFlow API");

app.Run();

// ApplicationUser class (already exists from previous challenge)
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

// TODO: Create AuditLog entity
// - Id, Action, AdminUserId, TargetUserId, Details, IpAddress, Timestamp

// TODO: Create ApplicationDbContext with AuditLog DbSet

// TODO: Create IUserManagementService interface and implementation
// Methods: GetUsersAsync, GetUserByIdAsync, UpdateUserStatusAsync, etc.

// TODO: Create IAuditLogService for consistent audit logging
// Methods: LogActionAsync, GetAuditLogsAsync
