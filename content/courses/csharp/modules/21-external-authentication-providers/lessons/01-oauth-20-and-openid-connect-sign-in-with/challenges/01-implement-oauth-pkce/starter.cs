using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// TODO: Configure authentication services
// 1. Add Authentication with:
//    - DefaultScheme: CookieAuthenticationDefaults.AuthenticationScheme
//    - DefaultChallengeScheme: OpenIdConnectDefaults.AuthenticationScheme
//
// 2. Add Cookie authentication with:
//    - LoginPath = "/auth/login"
//    - LogoutPath = "/auth/logout"
//    - Cookie.HttpOnly = true
//    - Cookie.SameSite = SameSiteMode.Lax
//    - ExpireTimeSpan = TimeSpan.FromHours(8)
//
// 3. Add OpenID Connect with PKCE for a generic OAuth/OIDC provider:
//    - Authority from Configuration["OAuth:Authority"]
//    - ClientId from Configuration["OAuth:ClientId"]
//    - ClientSecret from Configuration["OAuth:ClientSecret"]
//    - ResponseType = "code"
//    - UsePkce = true (REQUIRED for security)
//    - SaveTokens = true
//    - GetClaimsFromUserInfoEndpoint = true
//    - TokenValidationParameters: ValidateIssuer = true, ValidateAudience = true
//    - Scope: openid, profile, email
//    - CallbackPath = "/signin-oidc"
//    - Events: OnRedirectToIdentityProvider - add a custom header "X-Client-Version: 1.0"

// TODO: Add Authorization services

var app = builder.Build();

// TODO: Add authentication middleware
// TODO: Add authorization middleware

// TODO: Create endpoints:
// 1. GET /auth/login - Challenge with OpenID Connect, redirect to /dashboard after login
//    - Include a state parameter to prevent CSRF
// 2. GET /auth/logout - Sign out from both cookie and OIDC provider
// 3. GET /dashboard - Protected endpoint returning:
//    - IsAuthenticated status
//    - User's Name claim (if available)
//    - User's Email claim (if available)
//    - List of all claims for debugging
// 4. GET /auth/token - Return the access token if available (for API calls)

app.Run();
