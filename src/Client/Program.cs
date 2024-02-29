using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddOpenIdConnect(opts =>
    {
        opts.Authority = "https://localhost:5001";
        opts.ClientId = "web";
        opts.ClientSecret = "secret";
        opts.ResponseType = OpenIdConnectResponseType.Code;
        opts.UsePkce = true;
        opts.Scope.Add("email");
        opts.Scope.Add("profile");
        opts.Scope.Add("offline_access");
        opts.Scope.Add("api1");
        opts.SaveTokens = true;
        opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.MapInboundClaims = false;
        opts.GetClaimsFromUserInfoEndpoint = true;
    })
    .AddCookie();

var app = builder.Build();

app.MapGet("/challenge/{scheme}", (string scheme) => TypedResults.Challenge(new AuthenticationProperties
{
    RedirectUri = "/callback",
    Items =
    {
        { "scheme", scheme }
    }
}, [OpenIdConnectDefaults.AuthenticationScheme]));

app.MapGet("/callback", async (HttpContext httpContext) =>
{
    var authResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    var user = authResult.Principal;
    var properties = authResult.Properties;

    var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();
    var provider = properties?.Items["scheme"];

    if (provider is not null)
        claims.Add(new { Type = "provider", Value = provider });
    var items = authResult.Properties.Items.Select(i => new { i.Key, i.Value });
    return TypedResults.Ok(new { claims, items });
});

app.Run();