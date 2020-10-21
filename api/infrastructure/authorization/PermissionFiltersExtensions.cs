using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.Models.DB;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.infrastructure.authorization
{
    public static class PermissionFiltersExtensions
    {
        #region Sheriff
        public static IQueryable<Sheriff> ApplyPermissionFilters(this IQueryable<Sheriff> query, ClaimsPrincipal currentUser)
        {
            var currentUserId = currentUser.CurrentUserId();
            var currentUserHomeLocationId = currentUser.HomeLocationId();

            if (currentUser.HasPermission(Permission.ViewProfilesInAllLocation)) 
                return query;

            if (currentUser.HasPermission(Permission.ViewProfilesInOwnLocation))
                return query.FilterUsersInHomeLocationAndLoanedWithin5Days(currentUserHomeLocationId);

            return currentUser.HasPermission(Permission.ViewOwnProfile) ? query.Where(s => s.Id == currentUserId) : query.Where(s => false);
        }

        private static IQueryable<Sheriff> FilterUsersInHomeLocationAndLoanedWithin5Days(this IQueryable<Sheriff> query, int homeLocationId)
        {
            var sevenDaysFromNow = DateTimeOffset.Now.AddDays(7).Date;
            var now = DateTimeOffset.Now.Date;

            return query.Where(s => s.HomeLocationId == homeLocationId ||
                             s.AwayLocation.Any(al =>
                                 al.LocationId == homeLocationId &&
                                 !(al.StartDate > sevenDaysFromNow || now > al.EndDate) &&
                                 al.ExpiryDate == null));
        }
        #endregion

        #region Location
        public static IQueryable<Location> ApplyPermissionFilters(this IQueryable<Location> query, ClaimsPrincipal currentUser, SheriffDbContext db)
        {
            var currentUserId = currentUser.CurrentUserId();
            var currentUserHomeLocationId = currentUser.HomeLocationId();
            var currentUserRegionId = db.Location.AsNoTracking().FirstOrDefault(l => l.Id == currentUserHomeLocationId)?.RegionId;

            if (currentUser.HasPermission(Permission.ViewProvince))
                return query;

            var viewRegion = currentUser.HasPermission(Permission.ViewRegion);
            var viewAssignedLocation = currentUser.HasPermission(Permission.ViewAssignedLocation);
            var viewHomeLocation = currentUser.HasPermission(Permission.ViewHomeLocation);

            //Not sure if we want to put some sort of time limit on this. 
            var assignedLocationIds = db.SheriffAwayLocation.AsNoTracking().Where(sal => sal.SheriffId == currentUserId
                                                                                         && sal.ExpiryDate == null).Select(s => s.LocationId).Distinct().ToList();

            return query.Where(loc =>
                (viewRegion && currentUserRegionId.HasValue && loc.RegionId == currentUserRegionId) ||
                (viewAssignedLocation && assignedLocationIds.Any(ali => ali == loc.Id)) ||
                (viewHomeLocation && loc.Id == currentUserHomeLocationId));
        }
        #endregion Location
    }
}
