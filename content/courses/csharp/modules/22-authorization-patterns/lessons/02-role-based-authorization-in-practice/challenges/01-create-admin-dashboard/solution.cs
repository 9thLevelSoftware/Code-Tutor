using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Services configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// Register custom services
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// ==================== ADMIN DASHBOARD ENDPOINTS ====================

// User Management Endpoints
app.MapGet("/api/admin/users", [Authorize(Roles = "Admin")] async (
    int pageNumber = 1,
    int pageSize = 10,
    IUserManagementService userService) =>
{
    var result = await userService.GetUsersAsync(pageNumber, pageSize);
    return Results.Ok(result);
}).WithName("GetUsers").WithTags("Admin - Users");

app.MapGet("/api/admin/users/{id}", [Authorize(Roles = "Admin")] async (
    string id,
    IUserManagementService userService) =>
{
    var user = await userService.GetUserByIdAsync(id);
    return user != null ? Results.Ok(user) : Results.NotFound();
}).WithName("GetUserById").WithTags("Admin - Users");

app.MapPut("/api/admin/users/{id}/status", [Authorize(Roles = "Admin")] async (
    string id,
    UpdateUserStatusRequest request,
    IUserManagementService userService,
    IHttpContextAccessor httpContextAccessor,
    IAuditLogService auditLogService) =>
{
    var currentUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    // Prevent self-lockout
    if (id == currentUserId && !request.IsActive)
    {
        await auditLogService.LogActionAsync("DISABLE_USER_FAILED", id, "Attempted self-lockout prevented");
        return Results.BadRequest(new { Error = "Cannot disable your own account" });
    }
    
    var result = await userService.UpdateUserStatusAsync(id, request.IsActive);
    
    if (result.Succeeded)
    {
        await auditLogService.LogActionAsync(
            request.IsActive ? "ENABLE_USER" : "DISABLE_USER", 
            id, 
            $"User account status changed to {(request.IsActive ? "active" : "inactive")}");
    }
    
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result.Errors);
}).WithName("UpdateUserStatus").WithTags("Admin - Users");

app.MapDelete("/api/admin/users/{id}", [Authorize(Roles = "Admin")] async (
    string id,
    IUserManagementService userService,
    IHttpContextAccessor httpContextAccessor,
    IAuditLogService auditLogService) =>
{
    var currentUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    // Prevent self-deletion
    if (id == currentUserId)
    {
        await auditLogService.LogActionAsync("DELETE_USER_FAILED", id, "Attempted self-deletion prevented");
        return Results.BadRequest(new { Error = "Cannot delete your own account" });
    }
    
    var result = await userService.SoftDeleteUserAsync(id);
    
    if (result.Succeeded)
    {
        await auditLogService.LogActionAsync("DELETE_USER", id, "User soft-deleted");
    }
    
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result.Errors);
}).WithName("DeleteUser").WithTags("Admin - Users");

// Role Management Endpoints
app.MapGet("/api/admin/roles", [Authorize(Roles = "Admin")] async (
    RoleManager<IdentityRole> roleManager) =>
{
    var roles = await roleManager.Roles.ToListAsync();
    return Results.Ok(roles.Select(r => new { r.Id, r.Name }));
}).WithName("GetRoles").WithTags("Admin - Roles");

app.MapPost("/api/admin/users/{id}/roles", [Authorize(Roles = "Admin")] async (
    string id,
    AddRoleRequest request,
    IUserManagementService userService,
    IAuditLogService auditLogService) =>
{
    var result = await userService.AddRoleToUserAsync(id, request.Role);
    
    if (result.Succeeded)
    {
        await auditLogService.LogActionAsync("ADD_ROLE", id, $"Role '{request.Role}' added");
    }
    
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result.Errors);
}).WithName("AddRoleToUser").WithTags("Admin - Roles");

app.MapDelete("/api/admin/users/{id}/roles/{role}", [Authorize(Roles = "Admin")] async (
    string id,
    string role,
    IUserManagementService userService,
    IAuditLogService auditLogService) =>
{
    // Prevent removing last admin
    var result = await userService.RemoveRoleFromUserAsync(id, role);
    
    if (result.Succeeded)
    {
        await auditLogService.LogActionAsync("REMOVE_ROLE", id, $"Role '{role}' removed");
    }
    
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result.Errors);
}).WithName("RemoveRoleFromUser").WithTags("Admin - Roles");

app.MapGet("/api/admin/users/{id}/permissions", [Authorize(Roles = "Admin")] async (
    string id,
    IUserManagementService userService) =>
{
    var permissions = await userService.GetUserPermissionsAsync(id);
    return Results.Ok(permissions);
}).WithName("GetUserPermissions").WithTags("Admin - Roles");

// Audit Logging Endpoints
app.MapGet("/api/admin/audit-logs", [Authorize(Roles = "Admin")] async (
    int pageNumber = 1,
    int pageSize = 20,
    IAuditLogService auditLogService) =>
{
    var logs = await auditLogService.GetAuditLogsAsync(pageNumber, pageSize);
    return Results.Ok(logs);
}).WithName("GetAuditLogs").WithTags("Admin - Audit");

// Dashboard Statistics
app.MapGet("/api/admin/stats", [Authorize(Roles = "Admin")] async (
    IUserManagementService userService) =>
{
    var stats = await userService.GetDashboardStatisticsAsync();
    return Results.Ok(stats);
}).WithName("GetDashboardStats").WithTags("Admin - Stats");

app.MapGet("/", () => "Admin Dashboard - ShopFlow API");

app.Run();

// ==================== ENTITY CLASSES ====================

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Action { get; set; } = string.Empty;
    public string AdminUserId { get; set; } = string.Empty;
    public string? TargetUserId { get; set; }
    public string Details { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// ==================== SERVICE INTERFACES ====================

public interface IUserManagementService
{
    Task<PaginatedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<OperationResult> UpdateUserStatusAsync(string userId, bool isActive);
    Task<OperationResult> SoftDeleteUserAsync(string userId);
    Task<OperationResult> AddRoleToUserAsync(string userId, string role);
    Task<OperationResult> RemoveRoleFromUserAsync(string userId, string role);
    Task<UserPermissionsDto> GetUserPermissionsAsync(string userId);
    Task<DashboardStatsDto> GetDashboardStatisticsAsync();
}

public interface IAuditLogService
{
    Task LogActionAsync(string action, string? targetUserId, string details);
    Task<PaginatedResult<AuditLogDto>> GetAuditLogsAsync(int pageNumber, int pageSize);
}

// ==================== SERVICE IMPLEMENTATIONS ====================

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PaginatedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize)
    {
        var query = _userManager.Users.Where(u => u.IsActive);
        var totalCount = await query.CountAsync();
        
        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                CreatedAt = user.LockoutEnd?.DateTime ?? DateTime.MinValue
            });
        }

        return new PaginatedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            Roles = roles.ToList()
        };
    }

    public async Task<OperationResult> UpdateUserStatusAsync(string userId, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return OperationResult.Failed("User not found");

        user.IsActive = isActive;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded 
            ? OperationResult.Success() 
            : OperationResult.Failed(result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<OperationResult> SoftDeleteUserAsync(string userId)
    {
        return await UpdateUserStatusAsync(userId, false);
    }

    public async Task<OperationResult> AddRoleToUserAsync(string userId, string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            return OperationResult.Failed($"Role '{role}' does not exist");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return OperationResult.Failed("User not found");

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded 
            ? OperationResult.Success() 
            : OperationResult.Failed(result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<OperationResult> RemoveRoleFromUserAsync(string userId, string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            return OperationResult.Failed($"Role '{role}' does not exist");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return OperationResult.Failed("User not found");

        // Prevent removing last admin
        if (role == "Admin")
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (admins.Count == 1 && admins.First().Id == userId)
                return OperationResult.Failed("Cannot remove the last admin role");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded 
            ? OperationResult.Success() 
            : OperationResult.Failed(result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<UserPermissionsDto> GetUserPermissionsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new UserPermissionsDto { UserId = userId, Roles = new List<string>(), EffectivePermissions = new List<string>() };

        var roles = await _userManager.GetRolesAsync(user);
        
        // Map roles to effective permissions
        var permissions = new List<string>();
        foreach (var role in roles)
        {
            permissions.AddRange(GetPermissionsForRole(role));
        }

        return new UserPermissionsDto
        {
            UserId = userId,
            Email = user.Email!,
            Roles = roles.ToList(),
            EffectivePermissions = permissions.Distinct().ToList()
        };
    }

    private static IEnumerable<string> GetPermissionsForRole(string role) => role switch
    {
        "Admin" => new[] { "users.view", "users.manage", "roles.manage", "system.configure", "audit.view" },
        "User" => new[] { "profile.view", "profile.edit", "orders.create", "orders.view" },
        "Guest" => new[] { "catalog.view" },
        _ => Array.Empty<string>()
    };

    public async Task<DashboardStatsDto> GetDashboardStatisticsAsync()
    {
        var adminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
        var userCount = (await _userManager.GetUsersInRoleAsync("User")).Count;
        var totalUsers = await _userManager.Users.CountAsync();
        var activeUsers = await _userManager.Users.CountAsync(u => u.IsActive);

        var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
        var newUsersThisWeek = await _userManager.Users
            .CountAsync(u => u.LockoutEnd > oneWeekAgo);

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            AdminCount = adminCount,
            UserCount = userCount,
            NewUsersThisWeek = newUsersThisWeek,
            InactiveUsers = totalUsers - activeUsers
        };
    }
}

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogActionAsync(string action, string? targetUserId, string details)
    {
        var adminUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        var log = new AuditLog
        {
            Action = action,
            AdminUserId = adminUserId,
            TargetUserId = targetUserId,
            Details = details,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedResult<AuditLogDto>> GetAuditLogsAsync(int pageNumber, int pageSize)
    {
        var query = _context.AuditLogs.OrderByDescending(l => l.Timestamp);
        var totalCount = await query.CountAsync();

        var logs = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new AuditLogDto
            {
                Id = l.Id,
                Action = l.Action,
                AdminUserId = l.AdminUserId,
                TargetUserId = l.TargetUserId,
                Details = l.Details,
                IpAddress = l.IpAddress,
                Timestamp = l.Timestamp
            })
            .ToListAsync();

        return new PaginatedResult<AuditLogDto>
        {
            Items = logs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}

// ==================== DTOs ====================

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class UserPermissionsDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> EffectivePermissions { get; set; } = new();
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string AdminUserId { get; set; } = string.Empty;
    public string? TargetUserId { get; set; }
    public string Details { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int AdminCount { get; set; }
    public int UserCount { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int InactiveUsers { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class OperationResult
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();

    public static OperationResult Success() => new() { Succeeded = true };
    public static OperationResult Failed(params string[] errors) => new() { Succeeded = false, Errors = errors };
}

// ==================== REQUEST RECORDS ====================

public record UpdateUserStatusRequest(bool IsActive);
public record AddRoleRequest(string Role);

// ==================== DB CONTEXT ====================

public class ApplicationDbContext : DbContext
{
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AdminUserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Details).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}
