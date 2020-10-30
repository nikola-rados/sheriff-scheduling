using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models.auth;
using SS.Db.models.scheduling;
using SS.Db.models.scheduling.notmapped;

namespace SS.Api.controllers.scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private ShiftService ShiftService { get; }
        private SheriffService SheriffService { get; }

        public ShiftController(ShiftService shiftService, SheriffService sheriffService)
        {
            ShiftService = shiftService;
            SheriffService = sheriffService;
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
        public async Task<ActionResult<ShiftDto>> AddShift(ShiftDto shiftDto)
        {
            var shift = await ShiftService.AddShift(shiftDto.Adapt<Shift>());
            return Ok(shift.Adapt<ShiftDto>());
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
        public async Task<ActionResult<ShiftDto>> AssignToShifts(List<int> id, Guid sheriff, bool overrideShift = false)
        {
            await ShiftService.AssignToShifts(id, sheriff, overrideShift);
            return NoContent();
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireShifts)]
        public async Task<ActionResult<ShiftDto>> RemoveShift(int id)
        {
            await ShiftService.RemoveShift(id);
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
        public async Task<ActionResult<ShiftAvailability>> GetAvailability(DateTimeOffset startDate, DateTimeOffset endDate, int locationId)
        {
            if (startDate >= endDate)
                throw new BusinessLayerException("Start date was on or after end date.");
            if (endDate.UtcDateTime.Subtract(startDate.UtcDateTime).TotalDays > 30)
                throw new BusinessLayerException("End date and start date are more than 30 days apart.");
       
            var sheriffs = await SheriffService.GetSheriffsForShiftAvailability(locationId, startDate, endDate);
            var existingShifts = await ShiftService.GetShiftsForSheriffs(sheriffs.Select(s => s.Id), startDate, endDate);

            var sheriffShiftAvailability = sheriffs.Select(s => new ShiftAvailability
            {
                StartDate = startDate,
                EndDate = endDate,
                Sheriff = s,
                SheriffId = s.Id,
                /*ShiftConflict = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                    .Select(offset => startDate.AddDays(offset))
                    .Select(d => new SheriffShiftConflict { Date = d }).ToList()*/
            });

            throw new NotImplementedException();
            return new ShiftAvailability();
        }
    }
}
