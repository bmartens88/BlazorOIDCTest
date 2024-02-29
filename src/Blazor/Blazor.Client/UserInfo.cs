using System.Security.Claims;

namespace Blazor.Client;

public sealed class UserInfo
{
    public const string UserIdClaimType = "sub";
    public const string NameClaimType = "name";
    public required string UserId { get; init; }
    public required string Name { get; init; }

    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        return new UserInfo
        {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            Name = GetRequiredClaim(principal, NameClaimType)
        };
    }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(UserIdClaimType, UserId), new Claim(NameClaimType, Name)],
            nameof(UserInfo),
            NameClaimType,
            null));
    }

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType)
    {
        return principal.FindFirst(claimType)?.Value ??
               throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
    }
}