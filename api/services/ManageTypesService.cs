using Microsoft.EntityFrameworkCore;
using SS.Api.models;
using SS.Api.models.db;
using SS.Api.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.models;
using ss.db.models;

namespace SS.Api.services
{
    public class ManageTypesService
    {
        SheriffDbContext _db;
        public ManageTypesService(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<int> Add(LookupCode courtRoleCode)
        {
            _db.LookupCode.Add(courtRoleCode);
            await _db.SaveChangesAsync();
            return courtRoleCode.Id;
        }

        public async Task<List<LookupCode>> GetAll(LookupTypes? codeType, int? locationId)
        {
            return await _db.LookupCode.Where(lc => (codeType == null || lc.Type == codeType) && (locationId == null || lc.LocationId == locationId)).ToListAsync();
            //Different code path for CourtRooms, as it uses the JC-Interface. 
            if (codeType == LookupTypes.CourtRoom)
            {
            }
            else
            {
            }
        }

        public async ValueTask<LookupCode> Find(int id)
        {
            return await _db.LookupCode.FindAsync(id);
        }

        public async Task<bool> Update(LookupCode lookupCode)
        {
            _db.LookupCode.Update(lookupCode);
            return (await _db.SaveChangesAsync() > 0);
        }

        public async Task Remove(int id)
        {
            _db.LookupCode.Remove(_db.LookupCode.Single(crc => crc.Id == id));
            await _db.SaveChangesAsync();
        }
    }
}
