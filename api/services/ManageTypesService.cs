using Microsoft.EntityFrameworkCore;
using SS.Api.models.db;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public ManageTypesService(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<LookupCode> Add(LookupCode lookupCode)
        {
            //This needs to be associated, otherwise EF will create it. 
            if (lookupCode.Location != null)
                lookupCode.Location = await _db.Location.FindAsync(lookupCode.Location.Id);

            await _db.LookupCode.AddAsync(lookupCode);
            await _db.SaveChangesAsync();
            return lookupCode;
        }

        public async Task<List<LookupCode>> GetAll(LookupTypes? codeType, int? locationId)
        {
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
            //This needs to be associated, otherwise EF will create it. 
            if (lookupCode.Location != null)
                lookupCode.Location = await _db.Location.FindAsync(lookupCode.Location.Id);

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
