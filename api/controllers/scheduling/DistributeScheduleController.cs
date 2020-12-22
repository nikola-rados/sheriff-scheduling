using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.controllers.scheduling
{
    [Route("api/[controller]")]
    public class DistributeScheduleController : ControllerBase
    {
        private DistributeScheduleService DistributeScheduleService { get; }
        private ShiftService ShiftService { get; }
        private SheriffDbContext Db { get; }

        public DistributeScheduleController(DistributeScheduleService distributeSchedule, ShiftService shiftService,  SheriffDbContext db)
        {
            DistributeScheduleService = distributeSchedule;
            ShiftService = shiftService;
            Db = db;
        }

        [HttpGet("sheriffs")]
        [PermissionClaimAuthorize(perm: Permission.ViewDistributeSchedule)]
        public async Task<ActionResult<List<ShiftAvailabilityDto>>> GetDistributeScheduleForSheriffs(List<Guid> sheriffIds, DateTimeOffset start, DateTimeOffset end, bool includeWorkSection)
        {
            if (start >= end) return BadRequest("Start date was on or after end date.");
            if (end.Subtract(start).TotalDays > 30) return BadRequest("End date and start date are more than 30 days apart.");
            if (!User.HasPermission(Permission.ViewDutyRoster)) includeWorkSection = false;

            //Note: This has built in filtering for Sheriffs, based on permissions. 
            var shiftAvailability = await ShiftService.GetShiftAvailability(start, end, sheriffIds);
            var shiftsWithDuties = await DistributeScheduleService.GetDistributeSchedule(shiftAvailability, includeWorkSection);
            return Ok(shiftsWithDuties.Adapt<List<ShiftAvailabilityDto>>());
        }

        [HttpGet("location")]
        [PermissionClaimAuthorize(perm: Permission.ViewDistributeSchedule)]
        public async Task<ActionResult<List<ShiftAvailabilityDto>>> GetDistributeScheduleForLocation(int locationId, DateTimeOffset start, DateTimeOffset end, bool includeWorkSection)
        {
            if (start >= end) return BadRequest("Start date was on or after end date.");
            if (end.Subtract(start).TotalDays > 30) return BadRequest("End date and start date are more than 30 days apart.");
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();
            if (!User.HasPermission(Permission.ViewDutyRoster)) includeWorkSection = false;

            var shiftAvailability = await ShiftService.GetShiftAvailability(start, end, locationId: locationId);
            var shiftsWithDuties = await DistributeScheduleService.GetDistributeSchedule(shiftAvailability, includeWorkSection);
            return Ok(shiftsWithDuties.Adapt<List<ShiftAvailabilityDto>>());
        }
    }
}
