using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models.auth;
using SS.Db.models.scheduling;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private ScheduleService ScheduleService { get; }

        public ScheduleController(ScheduleService scheduleService)
        {
            ScheduleService = scheduleService;
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public async Task<ActionResult<ShiftDto>> AddShift(ShiftDto shiftDto)
        {
            var shift = await ScheduleService.AddShift(shiftDto.Adapt<Shift>());
            return Ok(shift.Adapt<ShiftDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditShifts)]
        public void UpdateShift()
        {
            //Time
            //WorkSection
            //Anticipated Assignment
            //Sheriff
        }

        [HttpPut]
        [Route("assign")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public void AssignToShift()
        {

        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireShifts)]
        public void RemoveShift()
        {

        }

        [HttpPost]
        [Route("import")]
        [PermissionClaimAuthorize(perm: Permission.ImportShifts)]
        public void ImportShift(bool includeSheriffs)
        {

        }
    }
}
