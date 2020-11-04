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
        [PermissionClaimAuthorize(perm: "GetDuties")]
        public async Task<ActionResult<List<DutyDto>>> GetDuties(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            var duties = await DutyRosterService.GetDuties(locationId, start, end);
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: "AddDuties")]
        public async Task<ActionResult<List<DutyDto>>> AddDutiesAsync(List<DutyDto> newDuties)
        {
            var duties = await DutyRosterService.AddDuties(newDuties.Adapt<List<Duty>>());
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: "EditDuties")]
        public async Task<ActionResult<List<DutyDto>>> UpdateDutiesAsync(List<DutyDto> editDuties)
        {
            var duties = await DutyRosterService.UpdateDuties(editDuties.Adapt<List<Duty>>());
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: "AssignDuties")]
        [Route("assign")]
        public async Task<ActionResult> AssignDuty(int id, int? shiftId)
        {
            await DutyRosterService.AssignDuty(id, shiftId);
            return NoContent();
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: "ExpireDuties")]
        public async Task<ActionResult> ExpireDuties(List<int> ids)
        {
            await DutyRosterService.ExpireDuties(ids);
            return NoContent();
        }
    }
}
