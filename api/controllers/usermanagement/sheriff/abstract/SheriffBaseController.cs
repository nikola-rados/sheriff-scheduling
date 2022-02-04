using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models;
using System;
using System.Threading.Tasks;

namespace SS.Api.controllers.usermanagement.sheriff
{
    public abstract class SheriffBaseController : ControllerBase
    {
        #region Constants
        protected const string CouldNotFindSheriffError = "Couldn't find sheriff.";
        public const string CouldNotFindSheriffEventError = "Couldn't find sheriff event.";
        #endregion

        #region Properties
        protected SheriffService SheriffService { get; }
        protected SheriffDbContext Db { get; }
        protected ShiftService ShiftService { get; }
        protected DutyRosterService DutyRosterService { get; }
        #endregion

        #region Constructor
        public SheriffBaseController(SheriffService sheriffService, UserService userUserService, SheriffDbContext db, ShiftService shiftService, DutyRosterService dutyRosterService) : base()
        {
            SheriffService = sheriffService;
            DutyRosterService = dutyRosterService;
            Db = db;
            ShiftService = shiftService;
        }
        #endregion

        #region Access Helpers
        protected async Task CheckForAccessToSheriffByLocation(Guid? id, string badgeNumber = null)
        {
            var savedSheriff = await SheriffService.GetSheriff(id, badgeNumber);
            if (savedSheriff == null) throw new NotFoundException(CouldNotFindSheriffError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedSheriff.HomeLocationId)) throw new NotAuthorizedException();
        }

        protected async Task CheckForAccessToSheriffByLocation<T>(int id) where T : SheriffEvent
        {
            var sheriffEvent = await SheriffService.GetSheriffEvent<T>(id);
            if (sheriffEvent == null) throw new NotFoundException(CouldNotFindSheriffEventError);
            var savedSheriff = await SheriffService.GetSheriff(sheriffEvent.SheriffId, null);
            if (savedSheriff == null) throw new NotFoundException(CouldNotFindSheriffError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedSheriff.HomeLocationId)) throw new NotAuthorizedException();
        }
        #endregion
    }
}
