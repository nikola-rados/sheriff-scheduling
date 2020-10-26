using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SS.Api.helpers;

namespace SS.Api.infrastructure.authorization
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private IMemoryCache Cache { get; }
        private ClaimsService ClaimsService { get; }
        private TimeSpan ClaimCachePeriod { get; }
        private bool _isTransformed;

        public ClaimsTransformer(IMemoryCache cache,  ClaimsService claimsService, IConfiguration configuration)
        {
            Cache = cache;
            ClaimsService = claimsService;
            ClaimCachePeriod = TimeSpan.Parse(configuration.GetNonEmptyValue("ClaimsCachePeriod"));
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated || _isTransformed) return await Task.FromResult(principal);
            var currentPrincipal = (ClaimsIdentity)principal.Identity;
            var currentClaims = currentPrincipal.Claims.ToList();

            var nameIdentifier = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!Cache.TryGetValue(nameIdentifier, out List<Claim> claims))
            {
                claims = await ClaimsService.GenerateClaims(currentClaims);
                Cache.Set(nameIdentifier, claims, DateTimeOffset.UtcNow.Add(ClaimCachePeriod));
            }
            currentPrincipal.AddClaims(claims);
            _isTransformed = true;
            return await Task.FromResult(principal);
        }
    }
}
