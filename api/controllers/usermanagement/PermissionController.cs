using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.controllers.usermanagement
{
    /// <summary>
    /// This just fetches our permissions, the permissions need to be determined at compile time. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
    public class PermissionController : ControllerBase
    {
        private SheriffDbContext Db { get; }

        public PermissionController(SheriffDbContext dbContext) {  Db = dbContext;  }

        [HttpGet]
        public async Task<ActionResult<List<PermissionDto>>> GetPermissions()
        {
            var permissions = await Db.Permission.AsNoTracking().ToListAsync();
            return Ok(permissions.Adapt<List<PermissionDto>>());
        }
    }
}
