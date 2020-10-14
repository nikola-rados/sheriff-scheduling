using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SS.Api.Helpers;
using SS.Api.Helpers.Extensions;
using SS.Db.models;

namespace SS.Api.infrastructure.authorization
{
    /// <summary>
    /// When Allow anonymous is used, the <see cref="ClaimsTransformer"/>
    /// Doesn't execute. 
    /// </summary>
    public class DevelopmentEnvironmentClaimsFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public DevelopmentEnvironmentClaimsFilter(IConfiguration configuration, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
                throw new Exception("This is not a development environment.");
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var identity = new ClaimsIdentity("Develop");
            context.HttpContext.User = new ClaimsPrincipal(identity);
            var claims = new List<Claim>();
            var roles = _configuration.GetNonEmptyValue("ImpersonateUser:Roles").Split(",").Select(s => s.Trim());
            var permissions = _configuration.GetNonEmptyValue("ImpersonateUser:Permissions").Split(",").Select(s => s.Trim());
            var userId = _configuration.GetNonEmptyValue("ImpersonateUser:UserId");
            var homeLocationId = _configuration.GetNonEmptyValue("ImpersonateUser:HomeLocationId");
            claims.AddRange(roles.SelectToList(r => new Claim(ClaimTypes.Role, r)));
            claims.AddRange(permissions.SelectToList(p => new Claim(CustomClaimTypes.Permission, p)));
            claims.Add(new Claim(CustomClaimTypes.UserId, userId));
            claims.Add(new Claim(CustomClaimTypes.HomeLocationId, homeLocationId));
            ((ClaimsIdentity)context.HttpContext.User.Identity).AddClaims(claims);
        }
    }
}
