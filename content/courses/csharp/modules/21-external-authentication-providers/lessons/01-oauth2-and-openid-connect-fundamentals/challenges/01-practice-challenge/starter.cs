using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// TODO: Create OAuth2Config class
// Properties: ClientId, ClientSecret, AuthorizationEndpoint, TokenEndpoint,
//             RedirectUri, Scope, UsePkce (default true)

// TODO: Create PkceData class (return type for PKCE generator)
// Properties: CodeVerifier, CodeChallenge, CodeChallengeMethod

// TODO: Create TokenResponse record
// Properties: AccessToken, RefreshToken, ExpiresIn, TokenType, CreatedAt
// Method: IsExpired property that checks expiration

// TODO: Create PkceGenerator class
// Method: Generate() returns PkceData with:
//   - CodeVerifier: 43-128 char random string (unreserved chars: A-Z, a-z, 0-9, -, _, ., ~)
//   - CodeChallenge: base64url(SHA256(codeVerifier))
//   - CodeChallengeMethod: "S256"

// TODO: Create OAuth2Client class
public class OAuth2Client
{
    private readonly OAuth2Config _config;
    private readonly HttpClient _httpClient;

    public OAuth2Client(OAuth2Config config, HttpClient? httpClient = null)
    {
        _config = config;
        _httpClient = httpClient ?? new HttpClient();
    }

    // TODO: Implement BuildAuthorizationUrl method
    // Parameters: state (string), pkceData (PkceData?, optional)
    // Returns: string URL with all OAuth2 parameters
    // Must include: response_type=code, client_id, redirect_uri (encoded), scope (encoded), state
    // If PKCE enabled: include code_challenge and code_challenge_method

    // TODO: Implement ExchangeCodeForTokensAsync method
    // Parameters: code (string), codeVerifier (string?)
    // Returns: Task<TokenResponse>
    // POST to TokenEndpoint with form data:
    //   grant_type=authorization_code, code, redirect_uri, client_id, client_secret
    //   If codeVerifier provided: include code_verifier
    // Parse JSON response and return TokenResponse

    // TODO: Implement ValidateState method
    // Parameters: returnedState (string), originalState (string)
    // Returns: bool
    // Should log warning if states don't match (possible CSRF)
}

// TODO: Create OAuthException class for OAuth2 errors
// Should include Error, ErrorDescription, ErrorUri properties

// Example usage (for reference - not part of TODOs):
// var config = new OAuth2Config {
//     ClientId = "shopflow-client",
//     ClientSecret = "secret123",
//     AuthorizationEndpoint = "https://auth.provider.com/authorize",
//     TokenEndpoint = "https://auth.provider.com/token",
//     RedirectUri = "https://shopflow.com/callback",
//     Scope = "inventory.read orders.write",
//     UsePkce = true
// };
// var client = new OAuth2Client(config);
// var pkce = PkceGenerator.Generate();
// var state = GenerateSecureState(); // Implement this helper
// var authUrl = client.BuildAuthorizationUrl(state, pkce);
// // ... redirect user to authUrl, handle callback ...
// var tokens = await client.ExchangeCodeForTokensAsync(code, pkce.CodeVerifier);
