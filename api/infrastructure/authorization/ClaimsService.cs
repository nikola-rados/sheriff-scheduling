using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Db.models;

namespace SS.Api.infrastructure.authorization
{
    /// <summary>
    /// This should load up our User context with claims from the database. 
    /// </summary>
    public class ClaimsService
    {
        private SheriffDbContext Db { get; }

        public ClaimsService(SheriffDbContext dbContext)
        {
            Db = dbContext;
        }

        public async Task<List<Claim>> GenerateClaims(List<Claim> currentClaims)
        {
            var claims = new List<Claim>();
            var idirName = currentClaims.GetIdirUserName();
            var idirId = currentClaims.GetIdirId();

            //Match by IdirID (already logged into SSO before) or Idir with no IdirID (created, but hasn't logged in yet).
            //Hopefully IdirID doesn't change when the base IdirName does (getting married / divorced etc).
            var user = await Db.User.AsSingleQuery().Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.IdirId == idirId || !u.IdirId.HasValue && u.IdirName == idirName);

            if (user?.IsEnabled != true) 
                return claims;

            user.IdirId ??= idirId;
            user.LastLogin = DateTime.UtcNow;
            user.KeyCloakId = currentClaims.GetKeyCloakId();
            await Db.SaveChangesAsync();

            claims.AddRange(user.ActiveRoles.SelectToList(r => new Claim(ClaimTypes.Role, r.Role.Name)));
            claims.AddRange(user.Permissions.SelectToList(p => new Claim(CustomClaimTypes.Permission, p.Name)));
            claims.Add(new Claim(CustomClaimTypes.UserId, user.Id.ToString()));
            if (user.HomeLocationId.HasValue)
                claims.Add(new Claim(CustomClaimTypes.HomeLocationId, user.HomeLocationId.ToString()));
            return claims;
        }
    }
}
