using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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

            var nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_cache.TryGetValue(nameIdentifier, out List<Claim> claims))
            {
                claims = new List<Claim>();
                if (await _db.User.AnyAsync(u => u.Id == Guid.Parse(nameIdentifier) && !u.IsDisabled))
                {
                    claims.Add(new Claim(Permission.Login, "TRUE"));

                    //TODO remove this later. 
                    if (_hostEnvironment.IsDevelopment())
                    {
                        claims.Add(new Claim(Permission.IsAdmin, "TRUE"));
                    }
                }
                _cache.Set(nameIdentifier, claims, DateTimeOffset.UtcNow.AddMinutes(2));
            }
            currentPrincipal.AddClaims(claims);
            _isTransformed = true;
            return await Task.FromResult(principal);
        }
    }
}
