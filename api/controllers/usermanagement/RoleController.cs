using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto;
using SS.Api.models.dto.generated;
using SS.Api.services.usermanagement;
using SS.Db.models.auth;

namespace SS.Api.controllers.usermanagement
{
    /// <summary>
    /// Used to manage roles. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private RoleService RoleService { get; }
        public RoleController(RoleService roleService)  {  RoleService = roleService; }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewRoles)]
        public async Task<ActionResult<List<RoleDto>>> Roles()
        {
            var roles = await RoleService.Roles();
            return Ok(roles.Adapt<List<RoleDto>>());
        }

        [HttpGet]
        [Route("{id}")]
        [PermissionClaimAuthorize(perm: Permission.ViewRoles)]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await RoleService.Role(id);
            if (role == null) return NotFound();
            return Ok(role.Adapt<RoleDto>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult<RoleDto>> AddRole(AddRoleDto addRole)        
        {
            addRole.ThrowBusinessExceptionIfNull("AddRole was null");
            addRole.Role.ThrowBusinessExceptionIfNull("Role was null");
            if (!addRole.PermissionIds.Any()) throw new BusinessLayerException("Permission Ids was empty");
            
            var entity = addRole.Role.Adapt<Role>();
            var createdRole = await RoleService.AddRole(entity, addRole.PermissionIds);
            return Ok(createdRole.Adapt<RoleDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditRoles)]
        public async Task<ActionResult<RoleDto>> UpdateRole(UpdateRoleDto updateRole)        
        {
            updateRole.ThrowBusinessExceptionIfNull("AddRole was null");
            updateRole.Role.ThrowBusinessExceptionIfNull("Role was null");

            var entity = updateRole.Role.Adapt<Role>();
            var updatedRole = await RoleService.UpdateRole(entity, updateRole.PermissionIds);
            return Ok(updatedRole.Adapt<RoleDto>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.EditRoles)]
        public async Task<ActionResult> RemoveRole(int id)
        {
            await RoleService.RemoveRole(id);
            return NoContent();
        }
    }
}
