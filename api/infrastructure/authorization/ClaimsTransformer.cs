using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using SS.Api.services;
using SS.Db.models;

namespace SS.Api.infrastructure
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly SheriffDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _hostEnvironment;
        private bool _isTransformed;
        private bool _checkForAuthenticate = true;
        private readonly AuthService _authService;
        
        public ClaimsTransformer(SheriffDbContext db, IMemoryCache cache, IWebHostEnvironment hostEnvironment, AuthService authService)
        {
            _db = db;
            _cache = cache;
            _hostEnvironment = hostEnvironment;
            _authService = authService;
            _checkForAuthenticate = true;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated || _isTransformed) return await Task.FromResult(principal);
            var currentPrincipal = (ClaimsIdentity)principal.Identity;
            var currentClaims = currentPrincipal.Claims.ToList();

            var nameIdentifier = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!_cache.TryGetValue(nameIdentifier, out List<Claim> claims))
            {
                claims = await _authService.GenerateClaims(currentClaims);
                _cache.Set(nameIdentifier, claims, DateTimeOffset.UtcNow.AddMinutes(2));
            }
            currentPrincipal.AddClaims(claims);
            _isTransformed = true;
            return await Task.FromResult(principal);
        }
    }
}
