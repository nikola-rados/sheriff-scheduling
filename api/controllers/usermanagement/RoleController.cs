using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models.auth;

namespace SS.Api.controllers.usermanagement
{
    /// <summary>
    /// We need to be able to assign roles to permissions here. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(Role.SystemAdministrator)]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _service;
        public RoleController(RoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> Roles()
        {
            var roles = await _service.Roles();
            return Ok(roles.Adapt<List<RoleDto>>());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var roles = await _service.Role(id);
            return Ok(roles.Adapt<RoleDto>());
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> AddRole(RoleDto role)
        {
            var entity = role.Adapt<Role>();
            var createdRole = await _service.AddRole(entity);
            return Ok(createdRole.Adapt<RoleDto>());
        }

        [HttpPut]
        public async Task<ActionResult<RoleDto>> UpdateRole(RoleDto role)
        {
            var entity = role.Adapt<Role>();
            var updatedRole = await _service.UpdateRole(entity);
            return Ok(updatedRole.Adapt<RoleDto>());
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveRole(int id)
        {
            await _service.RemoveRole(id);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/assignPermissions")]
        public async Task<ActionResult> AssignPermissions(int roleId, List<int> permissionIds)
        {
            await _service.AssignPermissionsToRole(roleId, permissionIds);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/unassignPermissions")]
        public async Task<ActionResult> UnassignPermissions(int roleId, List<int> permissionIds)
        {
            await _service.UnassignPermissionsFromRole(roleId, permissionIds);
            return NoContent();
        }

    }
}
