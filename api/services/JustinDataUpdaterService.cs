using System.Linq;
using System.Threading.Tasks;
using db.models;
using JCCommon.Clients.LocationServices;
using Microsoft.EntityFrameworkCore;
using SS.Api.Helpers.Extensions;
using SS.Api.Models.DB;
using ss.db.models;
using SS.Db.models;

namespace SS.Api.services
{
    public class JustinDataUpdaterService
    {
        private readonly LocationServicesClient _locationClient;
        private readonly SheriffDbContext _db;
        public JustinDataUpdaterService(SheriffDbContext dbContext, LocationServicesClient locationClient)
        {
            _locationClient = locationClient;
            _db = dbContext;
        }


        public async Task SyncCourtRooms()
        {
            var courtRoomsLookups = await _locationClient.LocationsRoomsAsync();
            var courtRooms = courtRoomsLookups;

            //Map these to locationIDs. 
            /*await _db.LookupCode.UpsertRange(courtRooms)
                .On(v => new { v.Type, })
                .WhenMatched(cr => new LookupCode
                {

                })
                .RunAsync();*/
        }

        public async Task SyncLocations()
        {
            var locationLookups = await _locationClient.LocationsAsync(null, true, false);
            var locations = locationLookups.SelectToList(loc => new Location {JustinCode = loc.ShortDesc, Name = loc.LongDesc, AgencyId = loc.Code});

            await _db.Location.UpsertRange(locations)
                               .On(v => v.AgencyId)
                               .WhenMatched(l => new Location
                               {
                                    Name = l.Name,
                               })
                               .RunAsync();

            await _db.SaveChangesAsync();
        }

        public async Task SyncRegions()
        {
            var regionLookups = await _locationClient.RegionsAsync();
            foreach (var region in regionLookups)
            {
                var go = await _locationClient.RegionsRegionIdLocationsCodesAsync("1");
            }
            var regions = regionLookups.SelectToList(r => new db.models.Region { JustinId = r.RegionId, Name = r.RegionName });
            var locationLookups = await _locationClient.LocationsAsync(null, true, false);
            await _db.Region.UpsertRange(regions)
                .On(v => v.JustinId)
                .WhenMatched(r => new db.models.Region
                {
                    Name = r.Name
                })
                .RunAsync();

            await _db.SaveChangesAsync();
        }
    }
}
