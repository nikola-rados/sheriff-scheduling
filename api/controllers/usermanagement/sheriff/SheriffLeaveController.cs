using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;
using System.Threading.Tasks;

namespace SS.Api.controllers.usermanagement.sheriff
{
    [Route("api/sheriff")]
    [ApiController]
    public class SheriffLeaveController : SheriffBaseController
    {
        #region Constructor
        public SheriffLeaveController(SheriffService sheriffService, DutyRosterService dutyRosterService, ShiftService shiftService, UserService userUserService, SheriffDbContext db) :
        base(sheriffService, userUserService, db, shiftService, dutyRosterService) { }
        #endregion Constructor

        #region Methods
        [HttpPost]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffLeaveDto>> AddSheriffLeave(SheriffLeaveDto sheriffLeaveDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation(sheriffLeaveDto.SheriffId);

            var sheriffLeave = sheriffLeaveDto.Adapt<SheriffLeave>();
            var createdSheriffLeave = await SheriffService.AddSheriffLeave(DutyRosterService, ShiftService, sheriffLeave, overrideConflicts);
            return Ok(createdSheriffLeave.Adapt<SheriffLeaveDto>());
        }

        [HttpPut]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffLeaveDto>> UpdateSheriffLeave(SheriffLeaveDto sheriffLeaveDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation<SheriffLeave>(sheriffLeaveDto.Id);

            var sheriffLeave = sheriffLeaveDto.Adapt<SheriffLeave>();
            var updatedSheriffLeave = await SheriffService.UpdateSheriffLeave(DutyRosterService, ShiftService, sheriffLeave, overrideConflicts);
            return Ok(updatedSheriffLeave.Adapt<SheriffLeaveDto>());
        }

        [HttpDelete]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffLeave(int id, string expiryReason)
        {
            await CheckForAccessToSheriffByLocation<SheriffLeave>(id);

            await SheriffService.RemoveSheriffLeave(id, expiryReason);
            return NoContent();
        }
        #endregion Methods
    }
}
