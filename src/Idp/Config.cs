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
                IdentityServerConstants.StandardScopes.Address
            ],
            Enabled = true,
            AllowedGrantTypes = GrantTypes.Code
        }
    ];
}