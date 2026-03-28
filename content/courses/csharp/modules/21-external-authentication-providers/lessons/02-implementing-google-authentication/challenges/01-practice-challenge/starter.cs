using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// TODO: Add Authentication services
// 1. Add Authentication with:
//    - DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme
//    - DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme
// 2. Add Cookie authentication
// 3. Add Google authentication with:
//    - ClientId from Configuration["Authentication:Google:ClientId"]
//    - ClientSecret from Configuration["Authentication:Google:ClientSecret"]
//    - SaveTokens = true
//    - Scopes: openid, email, profile
//    - MapJsonKey for "picture" to "ProfileImage" claim

// TODO: Add Authorization services

var app = builder.Build();

// TODO: Add Authentication and Authorization middleware
// Order matters: UseAuthentication before UseAuthorization

// In-memory user store for this challenge
var users = new Dictionary<string, ShopFlowUser>();

// TODO: Create GET /auth/google/login endpoint
// Initiates Google OAuth challenge
// Should redirect to /auth/google/callback after authentication
// Support optional ?returnUrl= query parameter

// TODO: Create GET /auth/google/callback endpoint
// Handles OAuth callback from Google
// Steps:
// 1. Authenticate with Google scheme to get external principal
// 2. If authentication failed, redirect to /login-failed
// 3. Extract claims: email, name, given_name, family_name, ProfileImage (mapped from picture)
// 4. Get the access token using GetTokenAsync
// 5. Create or update ShopFlowUser in dictionary (keyed by email)
// 6. Sign in with Cookie scheme
// 7. Redirect to returnUrl from AuthenticationProperties or /dashboard

// TODO: Create GET /api/user/profile endpoint (protected)
// Returns current authenticated user's info:
// - Email, Name, ProfileImage from claims
// - GoogleAccessToken from saved tokens
// - Provider: "Google"
// - LoginTime

// TODO: Create POST /auth/logout endpoint
// Signs out the user from Cookie scheme
// Redirects to home page (/)

app.Run();

// TODO: Create ShopFlowUser model class
// Properties: Id (guid), Email, Name, ProfileImageUrl, GoogleId, 
//             LastLoginAt, CreatedAt, GoogleAccessToken

// TODO: Create UserProfileResponse record for API response
// Properties: Email, Name, ProfileImage, Provider, LoginTime, AccessTokenPreview
// AccessTokenPreview should show first 10 chars + "..." of token
