using Microsoft.AspNetCore.Identity;

namespace Idp.Model;

internal sealed class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string? Name { get; set; }
}