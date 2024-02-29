using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Idp;

internal static class Config
{
    internal static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email()
    ];

    internal static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("api1", "My API")
    ];

    internal static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("Weather.Get", "Get weather forecast")
        {
            Scopes = ["api1"]
        }
    ];

    internal static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "web",
            ClientSecrets = [new Secret("secret".Sha256())],
            RedirectUris = ["https://localhost:5003/signin-oidc"],
            PostLogoutRedirectUris = ["https://localhost:5003/signout-callback-oidc"],
            AllowOfflineAccess = true,
            RequirePkce = true,
            AllowedScopes =
            [
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.Address,
                "api1"
            ],
            Enabled = true,
            AllowedGrantTypes = GrantTypes.Code
        }
    ];
}