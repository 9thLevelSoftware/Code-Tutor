using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// TODO: Add Authentication services
// Configure Cookie as default scheme

// TODO: Add Google Authentication (already configured)
// Same as previous challenge

// TODO: Add Microsoft Account Authentication
// Use AddMicrosoftAccount() with:
// - ClientId from Configuration["Authentication:Microsoft:ClientId"]
// - ClientSecret from Configuration["Authentication:Microsoft:ClientSecret"]
// - Tenant = "common"
// - SaveTokens = true
// - Scopes: openid, email, profile, User.Read

// TODO: Add GitHub Authentication
// Use AddOAuth with scheme name "GitHub":
// - ClientId from Configuration["Authentication:GitHub:ClientId"]
// - ClientSecret from Configuration["Authentication:GitHub:ClientSecret"]
// - AuthorizationEndpoint: https://github.com/login/oauth/authorize
// - TokenEndpoint: https://github.com/login/oauth/access_token
// - UserInformationEndpoint: https://api.github.com/user
// - CallbackPath: /signin-github
// - SaveTokens = true
// - Scopes: read:user, user:email
// - MapJsonKey: id -> NameIdentifier, login -> Name, avatar_url -> ProfileImage
// - In OnCreatingTicket: fetch emails from /user/emails if email is null, add User-Agent header

// TODO: Add Authorization services

var app = builder.Build();

// TODO: Add Authentication and Authorization middleware

// User store with multiple provider support
var users = new Dictionary<string, ShopFlowUser>();

// Provider scheme mapping (for reference, implement in your solution)
// var providerSchemes = new Dictionary<string, string>
// {
//     ["google"] = "Google",
//     ["microsoft"] = MicrosoftAccountDefaults.AuthenticationScheme,
//     ["github"] = "GitHub"
// };

// TODO: Create GET /api/auth/providers endpoint
// Returns list of available providers with metadata
// Format: [{ "name": "google", "displayName": "Google", "icon": "google", "color": "#4285F4" }, ...]

// TODO: Create GET /api/auth/challenge/{provider} endpoint
// - Accept provider name in route (google, microsoft, github)
// - Accept returnUrl query parameter
// - Validate provider is in allowed list (google, microsoft, github)
// - Map to correct authentication scheme
// - Challenge with that scheme
// - Store returnUrl in AuthenticationProperties

// TODO: Create GET /api/auth/callback/{provider} endpoint
// Handles callback from ALL providers
// Steps:
// 1. Authenticate with the provider scheme
// 2. Check for errors (query contains "error")
// 3. Get the authentication result and principal
// 4. Extract email from claims (use different logic per provider if needed)
// 5. Extract name and profile image (handle different claim types per provider)
// 6. Get access token using GetTokenAsync
// 7. Find or create user by email
// 8. Add/update external login info for this provider
// 9. Sign in with Cookie scheme
// 10. Redirect to returnUrl from properties or /dashboard

// TODO: Create GET /api/user/linked-providers (protected)
// Returns list of linked providers for current user
// Format: { "email": "...", "primaryProvider": "...", "linkedProviders": ["google", "microsoft"] }

// TODO: Create POST /api/user/set-primary-provider/{provider} (protected)
// Sets the preferred login provider for the user

app.Run();

// TODO: Create ShopFlowUser model with multi-provider support
// Properties: Id, Email, Name, ProfileImageUrl, PrimaryProvider, CreatedAt, LastLoginAt
// Plus collection of ExternalLogin objects

// TODO: Create ExternalLogin model
// Properties: Provider, ProviderKey, AccessToken, RefreshToken, ExpiresAt, Scope
