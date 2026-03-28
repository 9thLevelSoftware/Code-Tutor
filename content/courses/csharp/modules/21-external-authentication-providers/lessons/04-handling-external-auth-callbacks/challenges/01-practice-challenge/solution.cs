using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.SaveTokens = true;
});

// Register cache and custom services
builder.Services.AddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()));
builder.Services.AddSingleton<IRateLimiter, MemoryCacheRateLimiter>();
builder.Services.AddSingleton<ISecurityAuditLogger, SecurityAuditLogger>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// In-memory stores
var users = new Dictionary<string, ShopFlowUser>();

// ========== Models & Enums ==========

public enum OAuthErrorType
{
    None,
    AccessDenied,
    InvalidRequest,
    InvalidScope,
    ServerError,
    TemporarilyUnavailable,
    InvalidState,
    ReplayDetected,
    RateLimitExceeded,
    TokenExchangeFailed
}

public record OAuthErrorClassification(
    OAuthErrorType ErrorType,
    string Description,
    bool IsRetryable,
    string UserMessage,
    LogLevel LogLevel);

public class AuthorizationState
{
    public string State { get; set; } = string.Empty;
    public string? CodeVerifier { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }
    public string? Provider { get; set; }
    public string? ClientIp { get; set; }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    public void MarkUsed() => Used = true;
}

public class ShopFlowUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool IsFirstLogin { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

// ========== Services ==========

public static class OAuthErrorClassifier
{
    private static readonly Dictionary<string, OAuthErrorClassification> Classifications = new()
    {
        ["access_denied"] = new OAuthErrorClassification(
            OAuthErrorType.AccessDenied,
            "User denied consent",
            IsRetryable: true,
            "You chose not to grant access. You can try again if you change your mind.",
            LogLevel.Information),
        
        ["invalid_request"] = new OAuthErrorClassification(
            OAuthErrorType.InvalidRequest,
            "Invalid or missing request parameters",
            IsRetryable: true,
            "There was a problem with the login request. Please try again.",
            LogLevel.Warning),
        
        ["invalid_scope"] = new OAuthErrorClassification(
            OAuthErrorType.InvalidScope,
            "Invalid scope requested",
            IsRetryable: false,
            "The requested permissions are not available. Please contact support.",
            LogLevel.Error),
        
        ["server_error"] = new OAuthErrorClassification(
            OAuthErrorType.ServerError,
            "OAuth provider server error",
            IsRetryable: true,
            "The login service is experiencing issues. Please try again in a few minutes.",
            LogLevel.Error),
        
        ["temporarily_unavailable"] = new OAuthErrorClassification(
            OAuthErrorType.TemporarilyUnavailable,
            "Provider temporarily unavailable",
            IsRetryable: true,
            "The login service is temporarily unavailable. Please try again shortly.",
            LogLevel.Warning),
        
        ["invalid_state"] = new OAuthErrorClassification(
            OAuthErrorType.InvalidState,
            "State parameter validation failed",
            IsRetryable: true,
            "Your login session expired or is invalid. Please try again.",
            LogLevel.Warning),
    };

    public static OAuthErrorClassification Classify(string? error)
    {
        if (string.IsNullOrEmpty(error))
            return new OAuthErrorClassification(OAuthErrorType.None, "No error", false, string.Empty, LogLevel.None);
        
        return Classifications.TryGetValue(error, out var classification) 
            ? classification 
            : new OAuthErrorClassification(
                OAuthErrorType.InvalidRequest, 
                $"Unknown error: {error}", 
                false, 
                "An unexpected error occurred. Please try again.",
                LogLevel.Warning);
    }
}

public interface ISecurityAuditLogger
{
    void LogCallbackAttempt(string correlationId, string provider, string clientIp, string userAgent, string? stateHash);
    void LogSecurityEvent(string correlationId, string eventType, string description, string clientIp, LogLevel level);
    void LogSuccess(string correlationId, string provider, string email, bool isNewUser, string clientIp);
    void LogFailure(string correlationId, string provider, OAuthErrorType errorType, string description, string clientIp);
}

public class SecurityAuditLogger : ISecurityAuditLogger
{
    private readonly ILogger<SecurityAuditLogger> _logger;

    public SecurityAuditLogger(ILogger<SecurityAuditLogger> logger)
    {
        _logger = logger;
    }

    public void LogCallbackAttempt(string correlationId, string provider, string clientIp, string userAgent, string? stateHash)
    {
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            _logger.LogInformation(
                "OAuth callback attempt: Provider={Provider}, ClientIp={ClientIp}, StateHash={StateHash}, UserAgent={UserAgent}",
                provider, clientIp, stateHash ?? "none", userAgent);
        }
    }

    public void LogSecurityEvent(string correlationId, string eventType, string description, string clientIp, LogLevel level)
    {
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            _logger.Log(level, "Security event: {EventType} - {Description} from {ClientIp}", eventType, description, clientIp);
        }
    }

    public void LogSuccess(string correlationId, string provider, string email, bool isNewUser, string clientIp)
    {
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            _logger.LogInformation(
                "OAuth success: Provider={Provider}, Email={Email}, IsNewUser={IsNewUser}, ClientIp={ClientIp}",
                provider, email, isNewUser, clientIp);
        }
    }

    public void LogFailure(string correlationId, string provider, OAuthErrorType errorType, string description, string clientIp)
    {
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            _logger.LogWarning(
                "OAuth failure: Provider={Provider}, Error={ErrorType}, Description={Description}, ClientIp={ClientIp}",
                provider, errorType, description, clientIp);
        }
    }
}

public interface IRateLimiter
{
    bool IsAllowed(string key, int maxAttempts, TimeSpan window);
    void RecordAttempt(string key);
}

public class MemoryCacheRateLimiter : IRateLimiter
{
    private readonly IMemoryCache _cache;

    public MemoryCacheRateLimiter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool IsAllowed(string key, int maxAttempts, TimeSpan window)
    {
        var attempts = _cache.Get<RateLimitEntry>(key);
        
        if (attempts == null)
            return true;
        
        if (DateTime.UtcNow - attempts.FirstAttempt > window)
        {
            // Window expired, reset
            _cache.Remove(key);
            return true;
        }
        
        return attempts.Count < maxAttempts;
    }

    public void RecordAttempt(string key)
    {
        var attempts = _cache.Get<RateLimitEntry>(key);
        
        if (attempts == null)
        {
            attempts = new RateLimitEntry { FirstAttempt = DateTime.UtcNow, Count = 0 };
        }
        
        attempts.Count++;
        _cache.Set(key, attempts, TimeSpan.FromMinutes(1));
    }

    private class RateLimitEntry
    {
        public DateTime FirstAttempt { get; set; }
        public int Count { get; set; }
    }
}

// ========== Endpoints ==========

// Generate correlation ID
static string GenerateCorrelationId() => Guid.NewGuid().ToString("N")[..16];

// Generate secure state
static string GenerateState()
{
    var bytes = RandomNumberGenerator.GetBytes(32);
    return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
}

// Get state hash for logging (don't log full state)
static string GetStateHash(string state) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(state)))[..16];

// Initiate OAuth flow
app.MapGet("/auth/initiate/{provider}", (
    string provider,
    IMemoryCache cache,
    ISecurityAuditLogger auditLogger,
    HttpContext ctx) =>
{
    var allowedProviders = new[] { "google" };
    if (!allowedProviders.Contains(provider.ToLower()))
    {
        return Results.BadRequest(new { Error = "Invalid provider" });
    }

    var correlationId = GenerateCorrelationId();
    var state = GenerateState();
    var clientIp = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    
    // Store state in cache
    var authState = new AuthorizationState
    {
        State = state,
        CorrelationId = correlationId,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        Used = false,
        Provider = provider,
        ClientIp = clientIp
    };
    
    cache.Set($"oauth:state:{state}", authState, TimeSpan.FromMinutes(10));
    
    auditLogger.LogCallbackAttempt(correlationId, provider, clientIp, ctx.Request.Headers.UserAgent.ToString(), GetStateHash(state));
    
    // Store correlation ID in cookie for tracking
    ctx.Response.Cookies.Append("oauth_correlation", correlationId, new CookieOptions 
    { 
        HttpOnly = true, 
        Secure = true, 
        SameSite = SameSiteMode.Strict,
        MaxAge = TimeSpan.FromMinutes(10)
    });

    return Results.Challenge(
        new AuthenticationProperties 
        { 
            RedirectUri = $"/auth/callback/{provider}",
            Items = { { "state", state }, { "correlationId", correlationId } }
        },
        new[] { provider.ToLower() == "google" ? GoogleDefaults.AuthenticationScheme : provider }
    );
});

// Secure callback handler
app.MapGet("/auth/callback/{provider}", async (
    string provider,
    IMemoryCache cache,
    IRateLimiter rateLimiter,
    ISecurityAuditLogger auditLogger,
    HttpContext ctx,
    ILogger<Program> logger) =>
{
    var allowedProviders = new[] { "google" };
    if (!allowedProviders.Contains(provider.ToLower()))
    {
        return Results.BadRequest(new { Error = "Invalid provider" });
    }

    var clientIp = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var correlationId = ctx.Request.Cookies["oauth_correlation"] ?? GenerateCorrelationId();
    var userAgent = ctx.Request.Headers.UserAgent.ToString();
    
    // Rate limiting check
    var rateLimitKey = $"ratelimit:callback:{clientIp}";
    if (!rateLimiter.IsAllowed(rateLimitKey, maxAttempts: 10, window: TimeSpan.FromMinutes(1)))
    {
        auditLogger.LogSecurityEvent(correlationId, "RateLimitExceeded", "Too many callback attempts", clientIp, LogLevel.Warning);
        return Results.Redirect("/auth/security-error");
    }
    rateLimiter.RecordAttempt(rateLimitKey);

    // Check for OAuth errors
    if (ctx.Request.Query.TryGetValue("error", out var errorValue))
    {
        var error = errorValue.ToString();
        var classification = OAuthErrorClassifier.Classify(error);
        
        auditLogger.LogSecurityEvent(correlationId, classification.ErrorType.ToString(), 
            classification.Description, clientIp, classification.LogLevel);
        
        var redirectUrl = classification.ErrorType switch
        {
            OAuthErrorType.AccessDenied => "/login?message=consent_denied",
            OAuthErrorType.TemporarilyUnavailable => "/login?message=provider_unavailable&retry=1",
            _ => "/login?message=provider_error&retry=1"
        };
        
        return Results.Redirect(redirectUrl);
    }

    // Retrieve stored state
    if (!ctx.Request.Query.TryGetValue("state", out var returnedState))
    {
        auditLogger.LogSecurityEvent(correlationId, "MissingState", "Callback missing state parameter", clientIp, LogLevel.Warning);
        return Results.Redirect("/auth/security-error");
    }

    var stateKey = $"oauth:state:{returnedState}";
    var authState = cache.Get<AuthorizationState>(stateKey);
    
    if (authState == null)
    {
        auditLogger.LogSecurityEvent(correlationId, "StateNotFound", "State not found in cache (expired or invalid)", clientIp, LogLevel.Warning);
        return Results.Redirect("/login?message=session_expired");
    }

    // Check for replay attack
    if (authState.Used)
    {
        auditLogger.LogSecurityEvent(correlationId, "ReplayDetected", "State already used - possible replay attack", clientIp, LogLevel.Error);
        return Results.Redirect("/auth/security-error");
    }

    // Validate state with constant-time comparison
    var stateBytes = Encoding.UTF8.GetBytes(returnedState.ToString());
    var storedStateBytes = Encoding.UTF8.GetBytes(authState.State);
    
    if (!CryptographicOperations.FixedTimeEquals(stateBytes, storedStateBytes))
    {
        auditLogger.LogSecurityEvent(correlationId, "InvalidState", "State parameter mismatch - possible CSRF", clientIp, LogLevel.Error);
        return Results.Redirect("/auth/security-error");
    }

    // Check expiration
    if (authState.IsExpired())
    {
        auditLogger.LogSecurityEvent(correlationId, "StateExpired", "State parameter expired", clientIp, LogLevel.Information);
        cache.Remove(stateKey);
        return Results.Redirect("/login?message=session_expired");
    }

    // Mark state as used (replay protection)
    authState.MarkUsed();
    cache.Set(stateKey, authState, TimeSpan.FromMinutes(10));

    // Log attempt
    auditLogger.LogCallbackAttempt(correlationId, provider, clientIp, userAgent, GetStateHash(authState.State));

    // Authenticate with Google
    var result = await ctx.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    
    if (!result.Succeeded)
    {
        auditLogger.LogFailure(correlationId, provider, OAuthErrorType.TokenExchangeFailed, 
            result.Failure?.Message ?? "Unknown error", clientIp);
        return Results.Redirect("/login?message=authentication_failed&retry=1");
    }

    // Extract user info
    var principal = result.Principal;
    var email = principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
    var name = principal.FindFirstValue(System.Security.Claims.ClaimTypes.Name);
    
    if (string.IsNullOrEmpty(email))
    {
        auditLogger.LogFailure(correlationId, provider, OAuthErrorType.InvalidRequest, "No email in claims", clientIp);
        return Results.Redirect("/login?message=email_required");
    }

    // Create or update user
    var isNewUser = !users.ContainsKey(email);
    if (isNewUser)
    {
        users[email] = new ShopFlowUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = name,
            IsFirstLogin = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    var user = users[email];
    user.LastLoginAt = DateTime.UtcNow;
    user.Name = name ?? user.Name;

    // Log success
    auditLogger.LogSuccess(correlationId, provider, email, isNewUser, clientIp);

    // Sign in with cookie
    var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(System.Security.Claims.ClaimTypes.Email, email),
        new(System.Security.Claims.ClaimTypes.Name, user.Name ?? email),
        new("CorrelationId", correlationId)
    };
    
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    
    // Sign out from external provider
    await ctx.SignOutAsync(GoogleDefaults.AuthenticationScheme);

    // Determine redirect based on user status
    var redirect = isNewUser 
        ? "/welcome?onboarding=true" 
        : $"/dashboard?welcome={(user.IsFirstLogin ? "first" : "back")}";
    
    user.IsFirstLogin = false;

    return Results.Redirect(redirect);
});

// Security error page (generic - don't leak info)
app.MapGet("/auth/security-error", () => Results.Ok(new 
{ 
    Error = "A security error occurred", 
    Message = "Your login session could not be validated. This may happen if you took too long to complete the login or if there was a network issue. Please try again.",
    Action = "Click below to return to the login page and try again."
}));

// Login page handler
app.MapGet("/login", (string? message, int? retry) =>
{
    var userMessage = message switch
    {
        "consent_denied" => "You chose not to grant access. You can try again if you change your mind.",
        "provider_unavailable" => "The login service is temporarily unavailable. Please try again shortly.",
        "provider_error" => "There was a problem with the login service. Please try again.",
        "session_expired" => "Your login session expired. Please try again.",
        "email_required" => "An email address is required to continue. Please try again and allow email access.",
        "authentication_failed" => "Authentication failed. Please try again.",
        _ => "Please sign in to continue."
    };

    return Results.Ok(new { Message = userMessage, CanRetry = retry == 1 });
});

// Dashboard
app.MapGet("/dashboard", (string? welcome) =>
{
    var message = welcome switch
    {
        "first" => "Welcome to ShopFlow! This is your first time here.",
        "back" => "Welcome back to ShopFlow!",
        _ => "Dashboard"
    };
    
    return Results.Ok(new { Message = message });
}).RequireAuthorization();

// Welcome page
app.MapGet("/welcome", (bool? onboarding) => Results.Ok(new 
{ 
    Message = "Welcome to ShopFlow!",
    Onboarding = onboarding == true 
}));

app.Run();
