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
    public static class PermissionDataFiltersExtensions
    {
        #region Sheriff
        public static IQueryable<Sheriff> ApplyPermissionFilters(this IQueryable<Sheriff> query, ClaimsPrincipal currentUser, DateTimeOffset start, DateTimeOffset end, SheriffDbContext db)
        {
            var currentUserId = currentUser.CurrentUserId();
            var homeLocationId = currentUser.HomeLocationId();
            var viewProvince = currentUser.HasPermission(Permission.ViewProvince);
            var viewRegion = currentUser.HasPermission(Permission.ViewRegion);
            var viewAssignedLocation = currentUser.HasPermission(Permission.ViewAssignedLocation);
            var viewHomeLocation = currentUser.HasPermission(Permission.ViewHomeLocation);

            if (viewProvince) 
                return query;

            var homeRegionId = db.Location.AsNoTracking().Where(s => viewRegion).FirstOrDefault(l => l.Id == homeLocationId)?.RegionId;
            var locationsWithinRegion = db.Location.AsNoTracking().Where(l => viewRegion && l.RegionId == homeRegionId).SelectToList(l => l.Id);

            var assignedLocationIds = db.SheriffAwayLocation.AsNoTracking().Where(sal =>
                    viewAssignedLocation && sal.SheriffId == currentUserId &&
                    !(sal.StartDate > end || start > sal.EndDate) && sal.ExpiryDate == null)
                .SelectDistinctToList(s => s.LocationId);

            return query.Where(s =>
                (viewRegion && homeRegionId.HasValue && s.HomeLocationId != null && locationsWithinRegion.Contains((int)s.HomeLocationId)) ||
                (viewAssignedLocation && assignedLocationIds.Any(ali => ali == s.HomeLocationId)) ||
                ((viewRegion || viewAssignedLocation || viewHomeLocation) && s.HomeLocationId == homeLocationId));
        }

        #endregion

        #region Location
        public static IQueryable<Location> ApplyPermissionFilters(this IQueryable<Location> query, ClaimsPrincipal currentUser, SheriffDbContext db)
        {
            var currentUserId = currentUser.CurrentUserId();
            var currentUserHomeLocationId = currentUser.HomeLocationId();
            var currentUserRegionId = db.Location.AsNoTracking().FirstOrDefault(l => l.Id == currentUserHomeLocationId)?.RegionId;

            var viewProvince = currentUser.HasPermission(Permission.ViewProvince);
            var viewRegion = currentUser.HasPermission(Permission.ViewRegion);
            var viewAssignedLocation = currentUser.HasPermission(Permission.ViewAssignedLocation);
            var viewHomeLocation = currentUser.HasPermission(Permission.ViewHomeLocation);

            if (currentUser.HasPermission(Permission.ViewProvince))
                return query;

            //Not sure if we want to put some sort of time limit on this. 
            var assignedLocationIds = db.SheriffAwayLocation.AsNoTracking().Where(sal => sal.SheriffId == currentUserId
                                                                                         && sal.ExpiryDate == null).Select(s => s.LocationId).Distinct().ToList();
            return query.Where(loc =>
                (viewRegion && currentUserRegionId.HasValue && loc.RegionId == currentUserRegionId) ||
                (viewAssignedLocation && assignedLocationIds.Any(ali => ali == loc.Id)) ||
                ((viewProvince || viewRegion || viewAssignedLocation || viewHomeLocation) && loc.Id == currentUserHomeLocationId));
        }

        public static bool HasAccessToLocation(ClaimsPrincipal currentUser, SheriffDbContext db, int? locationId)
        {
            var currentUserId = currentUser.CurrentUserId();
            var currentUserHomeLocationId = currentUser.HomeLocationId();

            if (!locationId.HasValue || currentUser.HasPermission(Permission.ViewProvince)) return true;
            if (currentUser.HasPermission(Permission.ViewHomeLocation) && currentUserHomeLocationId == locationId) return true;

            if (currentUser.HasPermission(Permission.ViewRegion))
            {
                var currentUserRegionId = db.Location.AsNoTracking().FirstOrDefault(l => l.Id == currentUserHomeLocationId)?.RegionId;
                var locationRegionId = db.Location.AsNoTracking().FirstOrDefault(l => l.Id == locationId)?.RegionId;
                if (currentUserRegionId != null && currentUserRegionId == locationRegionId)
                    return true;
            }
            
            if (currentUser.HasPermission(Permission.ViewAssignedLocation))
            {
                //Not sure if we want to put some sort of time limit on this. 
                var assignedLocationIds = db.SheriffAwayLocation.AsNoTracking().Where(sal => sal.SheriffId == currentUserId
                    && sal.ExpiryDate == null).Select(s => s.LocationId).Distinct().ToList();
                if (assignedLocationIds.Contains(locationId))
                    return true;
            }
            return false;
        }
        #endregion Location
    }
}
