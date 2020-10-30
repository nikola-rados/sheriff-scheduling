using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Db.models.auth;
using SS.Db.models.scheduling;

namespace SS.Api.controllers.scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private AssignmentService AssignmentService { get; }

        public AssignmentController(AssignmentService assignmentService)
        {
            AssignmentService = assignmentService;
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewAssignments)]
        public async Task<ActionResult<List<AssignmentDto>>> GetAssignments(int locationId)
        {
            var assignments = await AssignmentService.GetAssignments(locationId);
            return Ok(assignments.Adapt<List<AssignmentDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAssignments)]
        public async Task<ActionResult<AssignmentDto>> AddAssignment(AssignmentDto assignmentDto)
        {
            var assignment = assignmentDto.Adapt<Assignment>();
            var createdAssignment = await AssignmentService.CreateAssignment(assignment);
            return Ok(createdAssignment.Adapt<AssignmentDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditAssignments)]
        public async Task<ActionResult<AssignmentDto>> UpdateAssignment(AssignmentDto assignmentDto)
        {
            var assignment = assignmentDto.Adapt<Assignment>();
            var updatedAssignment = await AssignmentService.UpdateAssignment(assignment);
            return Ok(updatedAssignment.Adapt<AssignmentDto>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireAssignments)]
        public async Task<ActionResult> RemoveAssignment(int id, string expiryReason)
        {
            await AssignmentService.RemoveAssignment(id, expiryReason);
            return NoContent();
        }
    }
}
