using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SS.Api.helpers;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Common.helpers.extensions;
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
        private IConfiguration Configuration { get; }

        public DistributeScheduleController(DistributeScheduleService distributeSchedule, ShiftService shiftService,  SheriffDbContext db, IConfiguration configuration)
        {
            DistributeScheduleService = distributeSchedule;
            ShiftService = shiftService;
            Db = db;
            Configuration = configuration;
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
            var shiftsWithDuties = await DistributeScheduleService.GetDistributeSchedule(shiftAvailability, includeWorkSection, start, end, locationId);

            if (!User.HasPermission(Permission.ViewAllFutureShifts) ||
                !User.HasPermission(Permission.ViewDutyRosterInFuture))
            {
                var location = await Db.Location.AsNoTracking().FirstOrDefaultAsync(l => l.Id == locationId);
                var timezone = location.Timezone;
                var currentDate = DateTimeOffset.UtcNow.ConvertToTimezone(timezone).DateOnly();

                if (!User.HasPermission(Permission.ViewAllFutureShifts))
                {
                    var shiftRestrictionDays = int.Parse(Configuration.GetNonEmptyValue("ViewShiftRestrictionDays"));
                    var endDateShift = currentDate.TranslateDateForDaylightSavings(timezone, shiftRestrictionDays + 1);
                    foreach (var sa in shiftsWithDuties)
                        sa.Conflicts = sa.Conflicts.WhereToList(c => c.Start < endDateShift);
                }

                if (!User.HasPermission(Permission.ViewDutyRosterInFuture))
                {
                    var dutyRestrictionHours =
                        float.Parse(Configuration.GetNonEmptyValue("ViewDutyRosterRestrictionHours"));
                    var endDateDuties =
                        currentDate.TranslateDateForDaylightSavings(timezone, hoursToShift: dutyRestrictionHours);
                    foreach (var sa in shiftsWithDuties)
                        sa.Conflicts.WhereToList(c => c.Start > endDateDuties)
                            .ForEach(c => c.WorkSection = null);
                }
            }

            return Ok(shiftsWithDuties.Adapt<List<ShiftAvailabilityDto>>());
        }
    }
}
