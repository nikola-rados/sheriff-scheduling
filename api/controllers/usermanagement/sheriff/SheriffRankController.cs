using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.controllers.usermanagement.sheriff;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;
using System.Threading.Tasks;

namespace SS.Api.controllers.usermanagement
{
    [Route("api/sheriff")]
    [ApiController]
    public class SheriffRankController : SheriffBaseController
    {
        #region Properties
        private SheriffRankService SheriffRankService { get; set; }
        #endregion

        #region Constructor
        public SheriffRankController(SheriffService sheriffService, SheriffRankService sheriffRankService, UserService userUserService, SheriffDbContext db, ShiftService shiftService, DutyRosterService dutyRosterService) :
        base (sheriffService, userUserService, db, shiftService, dutyRosterService) 
        {
            SheriffRankService = sheriffRankService;
        }
        #endregion

        #region Methods
        [HttpPost]
        [Route("rank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffRankDto>> AssignRank(AddSheriffRankDto addSheriffRank)
        {
            await CheckForAccessToSheriffByLocation(addSheriffRank.SheriffId);
            var sheriffRank = await SheriffRankService.AssignSheriffRank(addSheriffRank.Adapt<SheriffRank>());
            return Ok(sheriffRank.Adapt<SheriffRankDto>());
        }

        [HttpPut]
        [Route("rank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffRankDto>> UpdateRank(UpdateSheriffRankDto updateSheriffRank)
        {
            await CheckForAccessToSheriffByLocation(updateSheriffRank.SheriffId);
            var sheriffRank = await SheriffRankService.UpdateSheriffRank(updateSheriffRank.Adapt<SheriffRank>());
            return Ok(sheriffRank.Adapt<SheriffRankDto>());
        }
        #endregion
    }
}
