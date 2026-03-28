using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// TODO: Add Authentication with Cookie and Google

// TODO: Register IDistributedCache (Use AddDistributedMemoryCache for in-memory)

// TODO: Register IRateLimiter (custom interface - implement with MemoryCache)

// TODO: Register ISecurityAuditLogger (custom interface)

var app = builder.Build();

// TODO: Add required middleware (Authentication, etc.)

// In-memory stores for this challenge
var users = new Dictionary<string, ShopFlowUser>();
var authorizationStates = new Dictionary<string, AuthorizationState>(); // Simulate cache

// TODO: Create OAuthErrorType enum
// Values: AccessDenied, InvalidRequest, InvalidScope, ServerError, TemporarilyUnavailable, InvalidState

// TODO: Create OAuthErrorClassification class
// Properties: ErrorType, Description, IsRetryable, UserMessage, LogLevel

// TODO: Create OAuthErrorClassifier class with Classify method
// Takes error string, returns OAuthErrorClassification

// TODO: Create AuthorizationState class
// Properties: State, CodeVerifier, CorrelationId, CreatedAt, ExpiresAt, Used
// Method: IsExpired(), MarkUsed()

// TODO: Create ISecurityAuditLogger interface
// Methods: LogCallbackAttempt, LogSecurityEvent, LogSuccess, LogFailure

// TODO: Create IRateLimiter interface
// Methods: IsAllowed(string key, int maxAttempts, TimeSpan window), RecordAttempt(string key)

// TODO: Create callback endpoint GET /auth/callback/{provider}
// Comprehensive implementation:
// 1. Validate provider is allowed
// 2. Generate/retrieve correlation ID for tracing
// 3. Check for OAuth error query parameters
// 4. Classify error and handle appropriately (redirect with user-friendly message)
// 5. Validate rate limit for this IP
// 6. Retrieve state from cache/storage
// 7. Validate state exists and not expired
// 8. Validate state not already used (replay protection)
// 9. Validate state matches using constant-time comparison
// 10. Mark state as used immediately
// 11. Exchange code for tokens
// 12. Handle token exchange errors
// 13. Create/update user
// 14. Sign in with cookies
// 15. Redirect to appropriate destination based on user status
// 16. Log appropriate security events throughout

// TODO: Create helper endpoint GET /auth/initiate/{provider}
// - Generate secure state
// - Generate PKCE if needed
// - Store AuthorizationState in cache
// - Create correlation ID
// - Challenge with OAuth provider

// TODO: Create error display endpoints
// GET /auth/security-error - Generic security error (don't leak details)
// GET /login - With query param handling for different messages

app.Run();

// User model
public class ShopFlowUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool IsFirstLogin { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}
