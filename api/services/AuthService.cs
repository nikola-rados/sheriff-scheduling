using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.Helpers.Extensions;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.services
{
    /// <summary>
    /// This should handle all the permissions / roles / base user updates. 
    /// </summary>
    public class AuthService
    {
        private readonly SheriffDbContext _db;
        public AuthService(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// This will see if the user exists, if it doesn't exist it will create a new user object with the ID from
        /// the SSO. 
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <returns></returns>
        public async Task RequestAccess(ClaimsIdentity claimsIdentity)
        {
            var claims = claimsIdentity.Claims.ToList();
            var id = Guid.Parse(claims.GetValueByType(ClaimTypes.NameIdentifier));
            if (await _db.User.AnyAsync(u => u.Id == id))
                return;

            var user = new User
            {
                Id = id,
                IdirName = claims.GetValueByType("preferred_username"),
                IdirId = Guid.Parse(claims.GetValueByType("idir_userid")),
                FirstName = claims.GetValueByType(ClaimTypes.GivenName),
                LastName = claims.GetValueByType(ClaimTypes.Name),
                Email = claims.GetValueByType(ClaimTypes.Email),
                IsEnabled = false
            };
            await _db.User.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateUserLogin(ClaimsIdentity claimsIdentity)
        {
            var claims = claimsIdentity.Claims.ToList();
            var id = Guid.Parse(claims.GetValueByType(ClaimTypes.NameIdentifier));
            var user = await _db.User.FindAsync(id);
            if (user == null || !user.IsEnabled)
                return;
                
            user.LastLogin = DateTime.Now;
            await _db.SaveChangesAsync();
        }
    }
}
