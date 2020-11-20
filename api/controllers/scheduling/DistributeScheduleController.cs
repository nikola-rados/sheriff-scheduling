using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewDistributeSchedule)]
        public async Task<ActionResult<ShiftDto>> GetDistributeSchedule(DateTimeOffset start, DateTimeOffset end, int locationId, bool includeWorkSection)
        {
            if (start >= end) throw new BusinessLayerException("Start date was on or after end date.");
            if (end.Subtract(start).TotalDays > 30) throw new BusinessLayerException("End date and start date are more than 30 days apart.");

            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();

            var shiftAvailability = await ShiftService.GetShiftAvailability(locationId, start, end);
            var shiftsWithDuties = await DistributeScheduleService.GetDistributeSchedule(shiftAvailability, includeWorkSection);
            return Ok(shiftsWithDuties);
        }
    }
}
