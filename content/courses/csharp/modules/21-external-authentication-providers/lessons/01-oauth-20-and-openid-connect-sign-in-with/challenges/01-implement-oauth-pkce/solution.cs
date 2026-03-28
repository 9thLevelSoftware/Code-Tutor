using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration["OAuth:Authority"]
        ?? throw new InvalidOperationException("OAuth:Authority not configured");
    options.ClientId = builder.Configuration["OAuth:ClientId"]
        ?? throw new InvalidOperationException("OAuth:ClientId not configured");
    options.ClientSecret = builder.Configuration["OAuth:ClientSecret"]
        ?? throw new InvalidOperationException("OAuth:ClientSecret not configured");
    
    // PKCE is enabled by default in .NET 9, but we explicitly confirm it
    options.ResponseType = "code";
    options.UsePkce = true;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateTokenReplay = true
    };
    
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    
    options.CallbackPath = "/signin-oidc";
    
    options.Events.OnRedirectToIdentityProvider = context =>
    {
        context.HttpContext.Response.Headers.Append("X-Client-Version", "1.0");
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/login", () => Results.Challenge(
    new AuthenticationProperties 
    { 
        RedirectUri = "/dashboard",
        // Items dictionary can store custom state
        Items = { { "initiated_at", DateTime.UtcNow.ToString("O") } }
    },
    new[] { OpenIdConnectDefaults.AuthenticationScheme }
));

app.MapGet("/auth/logout", async (HttpContext ctx) =>
{
    // Sign out from local cookie
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    
    // Optionally trigger OIDC sign-out (ends session at provider)
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/" },
        new[] { OpenIdConnectDefaults.AuthenticationScheme }
    );
});

app.MapGet("/dashboard", (HttpContext ctx) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? false)
    {
        return Results.Unauthorized();
    }
    
    return Results.Ok(new
    {
        IsAuthenticated = true,
        Name = ctx.User.FindFirst(ClaimTypes.Name)?.Value,
        Email = ctx.User.FindFirst(ClaimTypes.Email)?.Value,
        Claims = ctx.User.Claims.Select(c => new { c.Type, c.Value }).ToList()
    });
}).RequireAuthorization();

app.MapGet("/auth/token", async (HttpContext ctx) =>
{
    var accessToken = await ctx.GetTokenAsync("access_token");
    var refreshToken = await ctx.GetTokenAsync("refresh_token");
    var idToken = await ctx.GetTokenAsync("id_token");
    
    if (string.IsNullOrEmpty(accessToken))
    {
        return Results.NotFound(new { Error = "No access token available. User may need to re-authenticate." });
    }
    
    return Results.Ok(new
    {
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        IdToken = idToken,
        ExpiresAt = await ctx.GetTokenAsync("expires_at")
    });
}).RequireAuthorization();

app.Run();
