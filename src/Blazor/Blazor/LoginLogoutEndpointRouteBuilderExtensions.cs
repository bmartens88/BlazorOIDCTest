using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Blazor;

internal static class LoginLogoutEndpointRouteBuilderExtensions
{
    internal static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("");

        group.MapGet("/login", (string? returnUrl) => TypedResults.Challenge(GetAuthProperties(returnUrl)))
            .AllowAnonymous();

        group.MapPost("/logout", ([FromForm] string? returnUrl) => TypedResults.SignOut(GetAuthProperties(returnUrl),
            [OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme]));

        return group;
    }

    private static AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        const string pathBase = "/";

        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = pathBase;
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        else if (returnUrl[0] != '/')
            returnUrl = $"{pathBase}{returnUrl}";

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}