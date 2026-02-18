using Microsoft.AspNetCore.Identity;

namespace Inredningsbutik.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? Street { get; set; }
public string? PostalCode { get; set; }
public string? City { get; set; }
public string? Country { get; set; }

}