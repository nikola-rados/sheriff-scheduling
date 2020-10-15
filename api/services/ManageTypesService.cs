using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JCCommon.Clients.LocationServices;
using SS.Api.Helpers.Extensions;
using ss.db.models;
using SS.Db.models;
using SS.Db.models.lookupcodes;

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
            return await _db.LookupCode.AsNoTracking().Where(lc =>
                    (codeType == null || lc.Type == codeType) && 
                    (locationId == null || lc.LocationId == locationId))
                .ToListAsync();
        }

        public async ValueTask<LookupCode> Find(int id)
        {
            return await _db.LookupCode.FindAsync(id);
        }

        public async Task<LookupCode> Expire(int id)
        {
            var entity = await _db.LookupCode.FindAsync(id);
            entity.ThrowBusinessExceptionIfNull($"Couldn't find lookup code with id: {id}");
            entity.ExpiryDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<LookupCode> Unexpire(int id)
        {
            var entity = await _db.LookupCode.FindAsync(id);
            entity.ThrowBusinessExceptionIfNull($"Couldn't find lookup code with id: {id}");
            entity.ExpiryDate = null;
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<LookupCode> Update(LookupCode lookupCode)
        {
            var savedLookup = await _db.LookupCode.FindAsync(lookupCode.Id);
            savedLookup.Location = await _db.Location.FindAsync(lookupCode.LocationId);

            _db.Entry(savedLookup).CurrentValues.SetValues(lookupCode);
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
