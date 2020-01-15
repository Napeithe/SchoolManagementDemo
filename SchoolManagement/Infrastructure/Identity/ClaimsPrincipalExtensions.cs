using System.Linq;
using System.Security.Claims;
using Model.Auth;

namespace SchoolManagement.Infrastructure.Identity
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }
        public static int GetUtc(this ClaimsPrincipal user)
        {
            return int.Parse(user?.Claims?.FirstOrDefault(x => x.Type == CustomClaims.UtcOffset)?.Value ?? "0");
        }

        public static string GetName(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        }

        public static string GetId(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static bool IsAuthenticated(this ClaimsPrincipal user)
        {
            return user?.Identity?.IsAuthenticated == true;
        }

        public static bool HasPermissionClaim(this ClaimsPrincipal user, string claim)
        {
            return user?.Claims?.Any(x =>
                       x.Type == CustomClaimTypes.Permission
                       && x.Value == claim) ?? false;
        }
    }
}
