using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JCCommon.Clients.LocationServices;
using SS.Api.helpers.extensions;
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
        private SheriffDbContext Db { get; }

        private readonly LocationServicesClient _locationClient;
        public ManageTypesService(SheriffDbContext dbContext, LocationServicesClient locationClient)
        {
            Db = dbContext;
            _locationClient = locationClient;
        }

        public async Task<LookupCode> Add(LookupCode lookupCode)
        {
            lookupCode.Location = await Db.Location.FindAsync(lookupCode.LocationId);

            await Db.LookupCode.AddAsync(lookupCode);
            await Db.SaveChangesAsync();
            return lookupCode;
        }

        public async Task<List<LookupCode>> GetAll(LookupTypes? codeType, int? locationId)
        {
            return await Db.LookupCode.AsNoTracking().Where(lc =>
                    (codeType == null || lc.Type == codeType) && 
                    (locationId == null || lc.LocationId == locationId))
                .ToListAsync();
        }

        public async ValueTask<LookupCode> Find(int id)
        {
            return await Db.LookupCode.FindAsync(id);
        }

        public async Task<LookupCode> Expire(int id)
        {
            var entity = await Db.LookupCode.FindAsync(id);
            entity.ThrowBusinessExceptionIfNull($"Couldn't find lookup code with id: {id}");
            entity.ExpiryDate = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return entity;
        }

        public async Task<LookupCode> Unexpire(int id)
        {
            var entity = await Db.LookupCode.FindAsync(id);
            entity.ThrowBusinessExceptionIfNull($"Couldn't find lookup code with id: {id}");
            entity.ExpiryDate = null;
            await Db.SaveChangesAsync();
            return entity;
        }

        public async Task<LookupCode> Update(LookupCode lookupCode)
        {
            var savedLookup = await Db.LookupCode.FindAsync(lookupCode.Id);
            savedLookup.Location = await Db.Location.FindAsync(lookupCode.LocationId);

            Db.Entry(savedLookup).CurrentValues.SetValues(lookupCode);
            await Db.SaveChangesAsync();
            return lookupCode;
        }

        public async Task Remove(int id)
        {
            Db.LookupCode.Remove(Db.LookupCode.Single(crc => crc.Id == id));
            await Db.SaveChangesAsync();
        }
    }
}
