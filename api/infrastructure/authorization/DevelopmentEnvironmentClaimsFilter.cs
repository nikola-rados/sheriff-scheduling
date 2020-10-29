using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SS.Api.helpers;
using SS.Api.helpers.extensions;

namespace SS.Api.infrastructure.authorization
{
    /// <summary>
    /// When Allow anonymous is used, the <see cref="ClaimsTransformer"/>
    /// Doesn't execute. 
    /// </summary>
    public class DevelopmentEnvironmentClaimsFilter : IAuthorizationFilter
    {
        private IConfiguration Configuration { get; }

        public DevelopmentEnvironmentClaimsFilter(IConfiguration configuration, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
                throw new Exception("This is not a development environment.");
            Configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var identity = new ClaimsIdentity("Develop");
            context.HttpContext.User = new ClaimsPrincipal(identity);
            var claims = new List<Claim>();
            var roles = Configuration.GetNonEmptyValue("ImpersonateUser:Roles").Split(",").Select(s => s.Trim());
            var permissions = Configuration.GetNonEmptyValue("ImpersonateUser:Permissions").Split(",").Select(s => s.Trim());
            var userId = Configuration.GetNonEmptyValue("ImpersonateUser:UserId");
            var homeLocationId = Configuration.GetNonEmptyValue("ImpersonateUser:HomeLocationId");
            claims.AddRange(roles.SelectToList(r => new Claim(ClaimTypes.Role, r)));
            claims.AddRange(permissions.SelectToList(p => new Claim(CustomClaimTypes.Permission, p)));
            claims.Add(new Claim(CustomClaimTypes.UserId, userId));
            claims.Add(new Claim(CustomClaimTypes.HomeLocationId, homeLocationId));
            ((ClaimsIdentity)context.HttpContext.User.Identity).AddClaims(claims);
        }
    }
}
