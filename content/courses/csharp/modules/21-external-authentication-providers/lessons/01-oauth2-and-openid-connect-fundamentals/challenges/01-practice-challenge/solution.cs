using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// OAuth2 Configuration Model
public class OAuth2Config
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool UsePkce { get; set; } = true;
}

// PKCE Data Container
public class PkceData
{
    public string CodeVerifier { get; set; } = string.Empty;
    public string CodeChallenge { get; set; } = string.Empty;
    public string CodeChallengeMethod { get; set; } = "S256";
}

// Token Response Model
public record TokenResponse(
    string AccessToken,
    string? RefreshToken,
    int ExpiresIn,
    string TokenType,
    DateTime CreatedAt = default)
{
    public DateTime CreatedAt { get; init; } = CreatedAt == default ? DateTime.UtcNow : CreatedAt;

    public bool IsExpired => DateTime.UtcNow >= CreatedAt.AddSeconds(ExpiresIn);

    public bool IsValid => !string.IsNullOrEmpty(AccessToken) && ExpiresIn > 0;
}

// PKCE Generator
public static class PkceGenerator
{
    private const string UnreservedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";

    public static PkceData Generate()
    {
        // Generate 43-128 character code verifier
        var verifierBytes = RandomNumberGenerator.GetBytes(32);
        var codeVerifier = Base64UrlEncode(verifierBytes);

        // Create code challenge = base64url(SHA256(codeVerifier))
        var challengeBytes = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        var codeChallenge = Base64UrlEncode(challengeBytes);

        return new PkceData
        {
            CodeVerifier = codeVerifier,
            CodeChallenge = codeChallenge,
            CodeChallengeMethod = "S256"
        };
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}

// OAuth2 Client Implementation
public class OAuth2Client
{
    private readonly OAuth2Config _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OAuth2Client>? _logger;

    public OAuth2Client(OAuth2Config config, HttpClient? httpClient = null, ILogger<OAuth2Client>? logger = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _httpClient = httpClient ?? new HttpClient();
        _logger = logger;
    }

    /// <summary>
    /// Builds the OAuth2 authorization URL with all required parameters
    /// </summary>
    public string BuildAuthorizationUrl(string state, PkceData? pkceData = null)
    {
        var sb = new StringBuilder();
        sb.Append(_config.AuthorizationEndpoint);
        sb.Append(_config.AuthorizationEndpoint.Contains('?') ? '&' : '?');
        sb.Append($"response_type=code");
        sb.Append($"&client_id={Uri.EscapeDataString(_config.ClientId)}");
        sb.Append($"&redirect_uri={Uri.EscapeDataString(_config.RedirectUri)}");
        sb.Append($"&scope={Uri.EscapeDataString(_config.Scope)}");
        sb.Append($"&state={Uri.EscapeDataString(state)}");

        if (_config.UsePkce && pkceData != null)
        {
            sb.Append($"&code_challenge={Uri.EscapeDataString(pkceData.CodeChallenge)}");
            sb.Append($"&code_challenge_method={Uri.EscapeDataString(pkceData.CodeChallengeMethod)}");
        }

        var url = sb.ToString();
        _logger?.LogDebug("Built authorization URL: {Url}", 
            url.Replace(_config.ClientId, "[REDACTED]"));

        return url;
    }

    /// <summary>
    /// Exchanges authorization code for access and refresh tokens
    /// </summary>
    public async Task<TokenResponse> ExchangeCodeForTokensAsync(string code, string? codeVerifier = null)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = _config.RedirectUri,
            ["client_id"] = _config.ClientId,
            ["client_secret"] = _config.ClientSecret
        };

        if (_config.UsePkce && !string.IsNullOrEmpty(codeVerifier))
        {
            parameters["code_verifier"] = codeVerifier;
        }

        var content = new FormUrlEncodedContent(parameters);

        _logger?.LogInformation("Exchanging authorization code for tokens at {Endpoint}", _config.TokenEndpoint);

        var response = await _httpClient.PostAsync(_config.TokenEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Token exchange failed: {StatusCode} - {Body}", 
                response.StatusCode, responseBody);

            var error = JsonSerializer.Deserialize<OAuthErrorResponse>(responseBody);
            throw new OAuthException(
                error?.Error ?? "unknown_error",
                error?.ErrorDescription ?? "Token exchange failed",
                error?.ErrorUri);
        }

        var tokenData = JsonSerializer.Deserialize<TokenResponseJson>(responseBody);

        if (tokenData?.AccessToken == null)
        {
            throw new OAuthException("invalid_response", "Token response missing access_token");
        }

        _logger?.LogInformation("Successfully obtained access token (expires in {ExpiresIn}s)", 
            tokenData.ExpiresIn);

        return new TokenResponse(
            tokenData.AccessToken,
            tokenData.RefreshToken,
            tokenData.ExpiresIn,
            tokenData.TokenType ?? "Bearer"
        );
    }

    /// <summary>
    /// Validates the state parameter to prevent CSRF attacks
    /// </summary>
    public bool ValidateState(string returnedState, string originalState)
    {
        if (string.IsNullOrEmpty(returnedState) || string.IsNullOrEmpty(originalState))
        {
            _logger?.LogWarning("State validation failed: null or empty state");
            return false;
        }

        // Use constant-time comparison to prevent timing attacks
        var isValid = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(returnedState),
            Encoding.UTF8.GetBytes(originalState));

        if (!isValid)
        {
            _logger?.LogWarning(
                "State validation failed - possible CSRF attack. Returned: {Returned}, Expected: {Expected}",
                returnedState.Substring(0, Math.Min(10, returnedState.Length)),
                originalState.Substring(0, Math.Min(10, originalState.Length)));
        }

        return isValid;
    }

    /// <summary>
    /// Generates a cryptographically secure state parameter
    /// </summary>
    public static string GenerateState()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}

// OAuth Exception
public class OAuthException : Exception
{
    public string Error { get; }
    public string? ErrorDescription { get; }
    public string? ErrorUri { get; }

    public OAuthException(string error, string? description = null, string? uri = null)
        : base($"OAuth error: {error} - {description}")
    {
        Error = error;
        ErrorDescription = description;
        ErrorUri = uri;
    }
}

// JSON deserialization models (internal)
internal class TokenResponseJson
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}

internal class OAuthErrorResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }

    [JsonPropertyName("error_uri")]
    public string? ErrorUri { get; set; }
}
