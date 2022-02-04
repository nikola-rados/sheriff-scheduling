using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class SheriffAwayLocationController : SheriffBaseController
    {
        #region Constructor
        // ReSharper disable once InconsistentNaming
        public SheriffAwayLocationController(SheriffService sheriffService, DutyRosterService dutyRosterService, ShiftService shiftService, UserService userUserService, SheriffDbContext db) 
            : base(sheriffService, userUserService, db, shiftService, dutyRosterService) { }
        #endregion Constructor

        #region Methods
        [HttpPost]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffAwayLocationDto>> AddSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation(sheriffAwayLocationDto.SheriffId);

            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var createdSheriffAwayLocation = await SheriffService.AddSheriffAwayLocation(DutyRosterService, ShiftService, sheriffAwayLocation, overrideConflicts);
            return Ok(createdSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
        }

        [HttpPut]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffAwayLocationDto>> UpdateSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation<SheriffAwayLocation>(sheriffAwayLocationDto.Id);

            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var updatedSheriffAwayLocation = await SheriffService.UpdateSheriffAwayLocation(DutyRosterService, ShiftService, sheriffAwayLocation, overrideConflicts);
            return Ok(updatedSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
        }

        [HttpDelete]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffAwayLocation(int id, string expiryReason)
        {
            await CheckForAccessToSheriffByLocation<SheriffAwayLocation>(id);

            await SheriffService.RemoveSheriffAwayLocation(id, expiryReason);
            return NoContent();
        }
        #endregion Methods
    }
}
