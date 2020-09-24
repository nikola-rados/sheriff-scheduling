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

        public async Task<LookupCode> Add(LookupCode courtRoleCode)
        {
            await _db.LookupCode.AddAsync(courtRoleCode);
            await _db.SaveChangesAsync();
            return courtRoleCode;
        }

        public async Task<List<LookupCode>> GetAll(LookupTypes? codeType, int? locationId)
        {
            return await _db.LookupCode.Where(lc =>
                    (codeType == null || lc.Type == codeType) && 
                    (locationId == null || lc.LocationId == locationId))
                .ToListAsync();
        }

        public async ValueTask<LookupCode> Find(int id)
        {
            return await _db.LookupCode.FindAsync(id);
        }

        public async Task<LookupCode> Update(LookupCode lookupCode)
        {
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
