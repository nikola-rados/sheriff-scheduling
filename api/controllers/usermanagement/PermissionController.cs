using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.controllers.usermanagement
{
    /// <summary>
    /// This just fetches our permissions, the permissions need to be determined at compile time. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(Role.SystemAdministrator)]
    public class PermissionController : ControllerBase
    {
        private readonly SheriffDbContext _db;
        public PermissionController(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<PermissionDto>> GetPermissions()
        {
            var permissions = await _db.Permission.ToListAsync();
            return Ok(permissions.Adapt<List<PermissionDto>>());
        }
    }
}
