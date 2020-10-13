using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JCCommon.Clients.LocationServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SS.Api.Helpers;
using SS.Api.Helpers.Extensions;
using SS.Api.Models.DB;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.lookupcodes;
using Region = db.models.Region;

namespace SS.Api.services.JC
{
    public class JCDataUpdaterService
    {
        private readonly ILogger _logger;
        private readonly LocationServicesClient _locationClient;
        private readonly SheriffDbContext _db;
        private IConfiguration Configuration { get; }
        private bool Expire { get; }

        public JCDataUpdaterService(SheriffDbContext dbContext, LocationServicesClient locationClient, IConfiguration configuration, ILogger<JCDataUpdaterService> logger)
        {
            _locationClient = locationClient;
            _db = dbContext;
            Configuration = configuration;
            Expire = Configuration.GetNonEmptyValue("JCSynchronizationExpire").Equals("true");
            _logger = logger;
        }

        public async Task SyncRegions()
        {
            var regionsDb = (await _locationClient.RegionsAsync()).SelectToList(r => new Region { JustinId = r.RegionId, Name = r.RegionName, CreatedById = User.SystemUser });
            await _db.Region.UpsertRange(regionsDb)
                            .On(v => v.JustinId)
                            .WhenMatched((r, rnew) => new Region
                            {
                                Name = rnew.Name,
                                JustinId = rnew.JustinId,
                                UpdatedOn = DateTime.UtcNow
                            })
                            .RunAsync();
            
            //Any regions that aren't on this list expire/disable for now. This is for regions that may have been deleted. 
            if (Expire)
            {
                var disableRegions = _db.Region.WhereToList(r => r.ExpiryDate == null && regionsDb.All(rdb => rdb.JustinId != r.JustinId));
                foreach (var disableRegion in disableRegions)
                {
                    _logger.LogDebug($"Expiring Region {disableRegion.Id}: {disableRegion.Name}");
                    disableRegion.ExpiryDate = DateTime.UtcNow;
                    disableRegion.UpdatedOn = DateTime.UtcNow;
                    disableRegion.UpdatedById = User.SystemUser;
                }
                await _db.SaveChangesAsync();
            }
        }

        public async Task SyncLocations()
        {
            var locationsDb = await GenerateLocationsAndLinkToRegions();

            await _db.Location.UpsertRange(locationsDb)
                               .On(v => v.AgencyId)
                               .WhenMatched((l, lnew) => new Location
                               {
                                   Name = lnew.Name,
                                   RegionId = lnew.RegionId,
                                   UpdatedOn = DateTime.UtcNow
                               })
                               .RunAsync();

            //Any Locations that aren't on this list expire/disable for now.  This is for locations that may have been deleted. 
            if (Expire)
            {
                var disableLocations = _db.Location.WhereToList(l => l.ExpiryDate == null && locationsDb.All(rdb => rdb.AgencyId != l.AgencyId ));
                foreach (var disableLocation in disableLocations)
                {
                    _logger.LogDebug($"Expiring Location {disableLocation.Id}: {disableLocation.Name}");
                    disableLocation.ExpiryDate = DateTime.UtcNow;
                    disableLocation.UpdatedOn = DateTime.UtcNow;
                    disableLocation.UpdatedById = User.SystemUser;
                }
                await _db.SaveChangesAsync();
            }
        }

        public async Task SyncCourtRooms()
        {
            var courtRoomsLookups = await _locationClient.LocationsRoomsAsync();
            //To list so we don't need to re-query on each select.
            var locations = _db.Location.ToList();
            var courtRooms = courtRoomsLookups.SelectToList(cr =>
                new ss.db.models.LookupCode
                {
                    Type = LookupTypes.CourtRoom,
                    Code = cr.Code,
                    LocationId = locations.FirstOrDefault(l => l.JustinCode == cr.Flex)
                        ?.Id,
                    CreatedById = User.SystemUser
                }).WhereToList(cr => cr.LocationId != null);

            //Some court rooms, don't have a location. This should probably be fixed in the JC-Interface
            var courtRoomNoLocation = courtRoomsLookups.WhereToList(crl => locations.All(l => l.JustinCode != crl.Flex));
            _logger.LogDebug("Court rooms without a location: ");
            _logger.LogDebug(JsonConvert.SerializeObject(courtRoomNoLocation));

            await _db.LookupCode.UpsertRange(courtRooms)
                 .On(v => new { v.Type, v.Code, v.LocationId })
                 .WhenMatched((cr, crNew) => new ss.db.models.LookupCode
                 {
                     Code = crNew.Code,
                     LocationId = crNew.LocationId,
                     UpdatedOn = DateTime.UtcNow
                 })
                 .RunAsync();

            //Any court rooms that aren't from this list, expire/disable for now. This is for CourtRooms that may have been deleted. 
            if (Expire)
            {
                var disableCourtRooms = _db.LookupCode.WhereToList(lc =>
                    lc.ExpiryDate == null &&
                    lc.Type == LookupTypes.CourtRoom &&
                    !courtRooms.Any(cr => cr.Type == lc.Type && cr.Code == lc.Code && cr.LocationId == lc.LocationId));

                foreach (var disableCourtRoom in disableCourtRooms)
                {
                    _logger.LogDebug($"Expiring CourtRoom {disableCourtRoom.Id}: {disableCourtRoom.Code}");
                    disableCourtRoom.ExpiryDate = DateTime.UtcNow;
                    disableCourtRoom.UpdatedOn = DateTime.UtcNow;
                    disableCourtRoom.UpdatedById = User.SystemUser;
                }
                await _db.SaveChangesAsync();
            }
        }

        private async Task<List<Location>> GenerateLocationsAndLinkToRegions()
        {
            var regionDictionary = new Dictionary<int, ICollection<int>>();
            //RegionsRegionIdLocationsCodesAsync returns a LIST of locationIds.
            foreach (var region in _db.Region)
                regionDictionary[region.Id] = await _locationClient.RegionsRegionIdLocationsCodesAsync(region.JustinId.ToString());

            //Reverse the dictionary and flatten.
            var locationToRegion = new Dictionary<string, int>();
            foreach (var (region, locationId) in regionDictionary.SelectMany(region => region.Value.Select(locationId => (region, locationId))))
                locationToRegion[locationId.ToString()] = region.Key;

            var locationWithoutRegion = new List<Location>();
            var locations = await _locationClient.LocationsAsync(null, true, false);
            var locationsDb = locations.SelectToList(loc =>
            {
                var regionId = locationToRegion.ContainsKey(loc.ShortDesc) ? locationToRegion[loc.ShortDesc] : null as int?;
                var location = new Location { JustinCode = loc.ShortDesc, Name = loc.LongDesc, AgencyId = loc.Code, RegionId = regionId, CreatedById = User.SystemUser };
                //Some locations don't have a region, this should be fixed in the JC-Interface. 
                if (regionId == null)
                    locationWithoutRegion.Add(location);

                return location;
            });

            _logger.LogDebug("Locations without a region: ");
            _logger.LogDebug(JsonConvert.SerializeObject(locationWithoutRegion));
            return locationsDb;
        }
    }
}
