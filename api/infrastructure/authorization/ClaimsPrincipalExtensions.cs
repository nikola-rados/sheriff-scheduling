using System;
using System.Linq;
using System.Security.Claims;

namespace SS.Api.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasPermissions(this ClaimsPrincipal user, params string[] permissions) => 
            user.HasClaim(c => c.Type == User.PERMISSION_CLAIM) && permissions.All(perm => user.HasClaim(User.PERMISSION_CLAIM, perm));

        public static bool IsInGroup(this ClaimsPrincipal user, string group) =>
            user.HasClaim(c => c.Type == ClaimTypes.GroupSid && c.Value.Equals(group, StringComparison.OrdinalIgnoreCase));
    }
}
