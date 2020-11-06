using System;
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
    public class DutyRosterController : ControllerBase
    {
        private DutyRosterService DutyRosterService { get; }
        public DutyRosterController(DutyRosterService dutyRosterService)
        {
            DutyRosterService = dutyRosterService;
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewDuties)]
        public async Task<ActionResult<List<DutyDto>>> GetDuties(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            var duties = await DutyRosterService.GetDuties(locationId, start, end);
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignDuties)]
        public async Task<ActionResult<List<DutyDto>>> AddDutiesAsync(List<SaveDutyDto> newDuties)
        {
            var duties = await DutyRosterService.AddDuties(newDuties.Adapt<List<Duty>>());
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditDuties)]
        public async Task<ActionResult<List<DutyDto>>> UpdateDutiesAsync(List<SaveDutyDto> editDuties)
        {
            var duties = await DutyRosterService.UpdateDuties(editDuties.Adapt<List<Duty>>());
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignDuties)]
        [Route("assign")]
        public async Task<ActionResult> AssignDuty(int id, int? shiftId)
        {
            await DutyRosterService.AssignDuty(id, shiftId);
            return NoContent();
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireDuties)]
        public async Task<ActionResult> ExpireDuties(List<int> ids)
        {
            await DutyRosterService.ExpireDuties(ids);
            return NoContent();
        }
    }
}
