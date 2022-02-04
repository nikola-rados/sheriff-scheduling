using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.controllers.usermanagement.sheriff
{
    [Route("api/sheriff")]
    [ApiController]
    public class SheriffTrainingController : SheriffBaseController
    {
        #region Constructor
        public SheriffTrainingController(SheriffService sheriffService, DutyRosterService dutyRosterService, ShiftService shiftService, UserService userUserService, SheriffDbContext db)
    : base(sheriffService, userUserService, db, shiftService, dutyRosterService) { }
        #endregion Constructor

        #region Methods
        [HttpPost]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffTrainingDto>> AddSheriffTraining(SheriffTrainingDto sheriffTrainingDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation(sheriffTrainingDto.SheriffId);

            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffTraining>();
            var createdSheriffTraining = await SheriffService.AddSheriffTraining(DutyRosterService, ShiftService, sheriffTraining, overrideConflicts);
            return Ok(createdSheriffTraining.Adapt<SheriffTrainingDto>());
        }

        [HttpPut]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffTrainingDto>> UpdateSheriffTraining(SheriffTrainingDto sheriffTrainingDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation<SheriffTraining>(sheriffTrainingDto.Id);

            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffTraining>();
            if (!User.HasPermission(Permission.EditPastTraining))
            {
                var savedSheriffTraining = Db.SheriffTraining.AsNoTracking().FirstOrDefault(st => st.Id == sheriffTrainingDto.Id);
                if (savedSheriffTraining?.EndDate <= DateTimeOffset.UtcNow)
                    throw new BusinessLayerException("No permission to edit training that has completed.");
            }

            var updatedSheriffTraining = await SheriffService.UpdateSheriffTraining(DutyRosterService, ShiftService, sheriffTraining, overrideConflicts);
            return Ok(updatedSheriffTraining.Adapt<SheriffTrainingDto>());
        }

        [HttpDelete]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffTraining(int id, string expiryReason)
        {
            await CheckForAccessToSheriffByLocation<SheriffTraining>(id);

            if (!User.HasPermission(Permission.RemovePastTraining))
            {
                var sheriffTraining = Db.SheriffTraining.AsNoTracking().FirstOrDefault(st => st.Id == id);
                if (sheriffTraining?.EndDate <= DateTimeOffset.UtcNow)
                    throw new BusinessLayerException("No permission to remove training that has completed.");
            }

            await SheriffService.RemoveSheriffTraining(id, expiryReason);
            return NoContent();
        }
        #endregion Methods
    }
}
