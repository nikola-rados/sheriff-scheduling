using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.infrastructure
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly SheriffDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _hostEnvironment;
        private bool _isTransformed;

        public ClaimsTransformer(SheriffDbContext db, IMemoryCache cache, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _cache = cache;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated || _isTransformed) return await Task.FromResult(principal);
            var currentPrincipal = (ClaimsIdentity)principal.Identity;

            var nameIdentifier = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!_cache.TryGetValue(nameIdentifier, out List<Claim> claims))
            {
                claims = new List<Claim>();

                //Enable all roles for now. 
                if (_hostEnvironment.IsDevelopment())
                {
                    claims.Add(new Claim(ClaimTypes.Role, Role.Sheriff));
                    claims.Add(new Claim(ClaimTypes.Role, Role.Administrator));
                    claims.Add(new Claim(ClaimTypes.Role, Role.SystemAdministrator));
                }

                var user = await _db.User.FindAsync(nameIdentifier);
                if (user != null  && user.IsEnabled)
                {
                    foreach (var userRole in user.Roles)
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                    //Todo add permissions here. 
                }
                _cache.Set(nameIdentifier, claims, DateTimeOffset.UtcNow.AddMinutes(2));
            }
            currentPrincipal.AddClaims(claims);
            _isTransformed = true;
            return await Task.FromResult(principal);
        }
    }
}
