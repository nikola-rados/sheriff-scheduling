using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SS.Api.Models.DB;
using SS.Db.models;

namespace SS.Api.infrastructure
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly SheriffDbContext _db;
        private readonly IMemoryCache _cache;
        private bool _isTransformed;

        public ClaimsTransformer(SheriffDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated || _isTransformed) return await Task.FromResult(principal);
            var currentPrincipal = (ClaimsIdentity)principal.Identity;

            var nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_cache.TryGetValue(nameIdentifier, out List<Claim> claims))
            {
                //Load claims here from DB. 
                claims = new List<Claim>();
                var canLogin = await _db.User.FirstOrDefaultAsync(u => u.Id == Guid.Parse(nameIdentifier) && !u.IsDisabled) != null;
                var c = new Claim("CAN_LOGIN", "TRUE");
                claims.Add(c);
                _cache.Set(nameIdentifier, claims, DateTimeOffset.UtcNow.AddMinutes(2));
            }
            currentPrincipal.AddClaims(claims);
            _isTransformed = true;
            return await Task.FromResult(principal);
        }
    }
}
