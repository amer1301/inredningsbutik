namespace Inredningsbutik.Infrastructure;

public static class AuthPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string SignedInUser = "SignedInUser";
    public const string CustomerOrAdmin = "CustomerOrAdmin";
}
