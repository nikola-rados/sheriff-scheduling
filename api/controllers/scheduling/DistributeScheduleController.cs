using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.controllers.scheduling
{
    public class DistributeScheduleController : ControllerBase
    {
        private DistributeScheduleService DistributeScheduleService { get; }
        private SheriffDbContext Db { get; }

        public DistributeScheduleController(DistributeScheduleService distributeSchedule, SheriffDbContext db)
        {
            DistributeScheduleService = distributeSchedule;
            Db = db;
        }

        [HttpGet]
        [Route("distributeSchedule")]
        [PermissionClaimAuthorize(perm: Permission.ViewDistributeSchedule)]
        public async Task<ActionResult<ShiftDto>> GetDistributeSchedule(DateTimeOffset start, DateTimeOffset end, int locationId, bool includeWorkSection)
        {
            var shiftsWithDuties = await DistributeScheduleService.GetDistributeSchedule(start, end, locationId, includeWorkSection);
            return Ok(shiftsWithDuties);
        }
    }
}
