using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SS.Api.helpers.extensions
{
    public static class ClaimExtensions
    {
        public static string GetValueByType(this List<Claim> claims, string type) =>
            claims.FirstOrDefault(c => c.Type == type)?.Value;

        public static bool HasPermissions(this ClaimsPrincipal user, params string[] permissions) =>
            user.HasClaim(c => c.Type == "APP_PERMISSION") && permissions.All(perm => user.HasClaim("APP_PERMISSION", perm));

        public static bool IsInGroup(this ClaimsPrincipal user, string group) =>
            user.HasClaim(c => c.Type == ClaimTypes.GroupSid && c.Value.Equals(group, StringComparison.OrdinalIgnoreCase));


    }
}
