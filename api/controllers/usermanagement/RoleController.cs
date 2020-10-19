using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto;
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
    public class RoleController : ControllerBase
    {
        private readonly RoleService _service;
        public RoleController(RoleService service)
        {
            _service = service;
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewRoles)]
        public async Task<ActionResult<List<RoleDto>>> Roles()
        {
            var roles = await _service.Roles();
            return Ok(roles.Adapt<List<RoleDto>>());
        }

        [HttpGet]
        [Route("{id}")]
        [PermissionClaimAuthorize(perm: Permission.ViewRoles)]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var roles = await _service.Role(id);
            return Ok(roles.Adapt<RoleDto>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult<RoleDto>> AddRole(AddRoleDto addRole)        
        {
            addRole.ThrowBusinessExceptionIfNull("AddRole was null");
            addRole.Role.ThrowBusinessExceptionIfNull("Role was null");
            addRole.PermissionIds.ThrowBusinessExceptionIfEmpty("Permission Ids was empty");
            
            var entity = addRole.Role.Adapt<Role>();
            var createdRole = await _service.AddRole(entity, addRole.PermissionIds);
            return Ok(createdRole.Adapt<RoleDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditRoles)]
        public async Task<ActionResult<RoleDto>> UpdateRole(UpdateRoleDto updateRole)        
        {
            updateRole.ThrowBusinessExceptionIfNull("AddRole was null");
            updateRole.Role.ThrowBusinessExceptionIfNull("Role was null");

            var entity = updateRole.Role.Adapt<Role>();
            var updatedRole = await _service.UpdateRole(entity, updateRole.PermissionIds);
            return Ok(updatedRole.Adapt<RoleDto>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.EditRoles)]
        public async Task<ActionResult> RemoveRole(int id)
        {
            await _service.RemoveRole(id);
            return NoContent();
        }
    }
}
