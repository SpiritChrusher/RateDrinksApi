using System.Security.Claims;

namespace RateDrinksApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        var isAdminClaim = user.FindFirst("IsAdmin")?.Value;
        return bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin;
    }
}
