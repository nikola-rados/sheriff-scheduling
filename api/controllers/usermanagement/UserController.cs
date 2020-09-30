using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.controllers.usermanagement
{
    /// <summary>
    /// This was made abstract, so it can be reused. The idea is you could take the User object and reuse with minimal changes in another project. 
    /// </summary>
    public abstract class UserController : ControllerBase
    {
        private readonly UserService _service;

        protected UserController(UserService userService)
        {
            _service = userService;
        }

        [HttpPut]
        [Route("{id}/assignRoles")]
        [AuthorizeRoles(Role.SystemAdministrator)]
        public async Task<ActionResult> AssignRoles(Guid id, List<int> roleIds)
        {
            await _service.AssignRolesToUser(id, roleIds);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/unassignRoles")]
        [AuthorizeRoles(Role.SystemAdministrator)]
        public async Task<ActionResult> UnassignRoles(Guid id, List<int> roleIds)
        {
            await _service.UnassignRoleFromUser(id, roleIds);
            return NoContent();
        }



        [HttpPut]
        [Route("{id}/enable")]
        [AuthorizeRoles(Role.Administrator, Role.SystemAdministrator)]
        public async Task<ActionResult<SheriffDto>> EnableUser(Guid id)
        {
            var user = await _service.EnableUser(id);
            return Ok(user.Adapt<SheriffDto>());
        }

        [HttpPut]
        [Route("{id}/disable")]
        [AuthorizeRoles(Role.Administrator, Role.SystemAdministrator)]
        public async Task<ActionResult<SheriffDto>> DisableUser(Guid id)
        {
            var user = await _service.DisableUser(id);
            return Ok(user.Adapt<SheriffDto>());
        }


    }
}
