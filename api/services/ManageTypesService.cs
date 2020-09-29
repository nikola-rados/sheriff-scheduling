using Microsoft.EntityFrameworkCore;
using SS.Api.models.db;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JCCommon.Clients.LocationServices;
using SS.Api.Helpers.Extensions;
using ss.db.models;
using SS.Db.models;

namespace SS.Api.services
{
    public class ManageTypesService
    {
        /// <summary>
        /// Handles ManageTypes or LookupCodes. 
        /// </summary>
        /// 
        readonly SheriffDbContext _db;

        private readonly LocationServicesClient _locationClient;
        public ManageTypesService(SheriffDbContext dbContext, LocationServicesClient locationClient)
        {
            _db = dbContext;
            _locationClient = locationClient;
        }

        public async Task<LookupCode> Add(LookupCode lookupCode)
        {
            lookupCode.Location = await _db.Location.FindAsync(lookupCode.LocationId);

            await _db.LookupCode.AddAsync(lookupCode);
            await _db.SaveChangesAsync();
            return lookupCode;
        }

        public async Task<List<LookupCode>> GetAll(LookupTypes? codeType, int? locationId)
        {

            if (codeType == LookupTypes.CourtRoom)
            {
                var locationRooms = await _locationClient.LocationsRoomsAsync();
                return locationRooms.SelectToList(lr => new LookupCode {Code = lr.Code, LocationId = _db.Location.FirstOrDefault(l => l.JustinCode == lr.Flex)?.Id });
            }
            return await _db.LookupCode.Where(lc =>
                    (codeType == null || lc.Type == codeType) && 
                    (locationId == null || lc.Location.Id == locationId))
                .ToListAsync();
        }

        public async ValueTask<LookupCode> Find(int id)
        {
            return await _db.LookupCode.FindAsync(id);
        }

        public async Task<LookupCode> Update(LookupCode lookupCode)
        {
            lookupCode.Location = await _db.Location.FindAsync(lookupCode.LocationId);

            _db.LookupCode.Update(lookupCode);
            await _db.SaveChangesAsync();
            return lookupCode;
        }

        public async Task Remove(int id)
        {
            _db.LookupCode.Remove(_db.LookupCode.Single(crc => crc.Id == id));
            await _db.SaveChangesAsync();
        }
    }
}
