using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SS.Api.helpers.extensions;
using SS.Api.Models.DB;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.infrastructure.authorization
{
    public static class PermissionFiltersExtensions
    {
        public static IQueryable<Sheriff> ApplyPermissionFilters(this IQueryable<Sheriff> query, ClaimsPrincipal currentUser)
        {
            var currentUserId = currentUser.CurrentUserId();
            var currentUserHomeLocationId = currentUser.HomeLocationId();
            var now = DateTimeOffset.Now.Date;
            var fiveDaysFromNow = DateTimeOffset.Now.AddDays(5).Date;

            if (currentUser.HasPermission(Permission.ViewProfilesInAllLocation)) return query;

            if (currentUser.HasPermission(Permission.ViewProfilesInOwnLocation))
                return query.Where(s => s.HomeLocationId == currentUserHomeLocationId ||
                                        s.AwayLocation.Any(al =>
                                            al.LocationId == currentUserHomeLocationId &&
                                            !(al.StartDate > fiveDaysFromNow || now > al.EndDate) &&
                                            al.ExpiryDate == null));

            if (currentUser.HasPermission(Permission.ViewOwnProfile)) return query.Where(s => s.Id == currentUserId);
            
            return query.Where(s => false);
        }

        public static IQueryable<Location> ApplyPermissionFilters(this IQueryable<Location> query)
        {
            return query;
        }
    }
}
