using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SS.Api.Helpers;
using SS.Api.services;
using SS.Db.models;

namespace SS.Api.infrastructure.authorization
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly IMemoryCache _cache;
        private bool _isTransformed;
        private readonly ClaimsService _claimsService;
        private readonly TimeSpan _claimCachePeriod;

        public ClaimsTransformer(IMemoryCache cache,  ClaimsService claimsService, IConfiguration configuration)
        {
            _cache = cache;
            _claimsService = claimsService;
            _claimCachePeriod = TimeSpan.Parse(configuration.GetNonEmptyValue("ClaimsCachePeriod"));
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated || _isTransformed) return await Task.FromResult(principal);
            var currentPrincipal = (ClaimsIdentity)principal.Identity;
            var currentClaims = currentPrincipal.Claims.ToList();

            var nameIdentifier = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!_cache.TryGetValue(nameIdentifier, out List<Claim> claims))
            {
                claims = await _claimsService.GenerateClaims(currentClaims);
                _cache.Set(nameIdentifier, claims, DateTimeOffset.UtcNow.Add(_claimCachePeriod));
            }
            currentPrincipal.AddClaims(claims);
            _isTransformed = true;
            return await Task.FromResult(principal);
        }
    }
}
