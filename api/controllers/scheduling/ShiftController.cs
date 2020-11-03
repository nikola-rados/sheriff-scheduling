using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Db.models.auth;
using SS.Db.models.scheduling;

namespace SS.Api.controllers.scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private ShiftService ShiftService { get; }

        public ShiftController(ShiftService shiftService)
        {
            ShiftService = shiftService;
        }

        #region Shift
        [HttpGet]
        [PermissionClaimAuthorize(AuthorizeOperation.And, Permission.ViewAllShifts, Permission.ViewMyShifts,
            Permission.ViewAllShiftsAtMyLocation)]
        public async Task<ActionResult<List<ShiftDto>>> GetShifts(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            var shifts = await ShiftService.GetShifts(locationId, start, end);
            return Ok(shifts.Adapt<List<ShiftDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public async Task<ActionResult<List<ShiftDto>>> AddShifts(List<ShiftDto> shiftDtos)
        {
            var shift = await ShiftService.AddShifts(shiftDtos.Adapt<List<Shift>>());
            return Ok(shift.Adapt<List<ShiftDto>>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditShifts)]
        public async Task<ActionResult<ShiftDto>> UpdateShift(ShiftDto shiftDto)
        {
            var shift = await ShiftService.UpdateShift(shiftDto.Adapt<Shift>());
            return Ok(shift.Adapt<ShiftDto>());
        }

        [HttpPut]
        [Route("assign")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public async Task<ActionResult<List<ShiftDto>>> AssignToShifts(List<int> id, Guid sheriff, bool overrideShift = false)
        {
            var assignToShifts = await ShiftService.AssignToShifts(id, sheriff, overrideShift);
            return Ok(assignToShifts.Adapt<List<ShiftDto>>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireShifts)]
        public async Task<ActionResult<ShiftDto>> ExpireShift(int id)
        {
            await ShiftService.ExpireShift(id);
            return NoContent();
        }

        [HttpPost]
        [Route("import")]
        [PermissionClaimAuthorize(perm: Permission.ImportShifts)]
        public async Task<ActionResult<List<ShiftDto>>> ImportWeeklyShifts(int locationId, bool includeSheriffs)
        {
            var shifts = await ShiftService.ImportWeeklyShifts(locationId, includeSheriffs);
            return Ok(shifts.Adapt<List<ShiftDto>>());
        }
        #endregion

        [HttpGet]
        [Route("shiftAvailability")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public async Task<ActionResult<List<ShiftAvailabilityDto>>> GetAvailability(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            if (start >= end)
                throw new BusinessLayerException("Start date was on or after end date.");
            if (end.Subtract(start).TotalDays > 30)
                throw new BusinessLayerException("End date and start date are more than 30 days apart.");

            var shiftAvailability = await ShiftService.GetShiftAvailability(locationId, start, end);
            return Ok(shiftAvailability.Adapt<List<ShiftAvailabilityDto>>());
        }
    }
}
