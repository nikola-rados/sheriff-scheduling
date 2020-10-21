using SS.Db.models.scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Db.models;

namespace SS.Api.services
{
    public class ScheduleService
    {
        private SheriffDbContext _db;
        public ScheduleService(SheriffDbContext db)
        {
            _db = db;
        }

        public async Task<Shift> AddShift()
        {
            return new Shift();
        }

        public async Task<Shift> UpdateShift()
        {
            return new Shift();
        }

        public async Task<Shift> AssignToShift()
        {
            return new Shift();
        }

        public async Task RemoveShift(int id)
        {
            //return new Shift();
        }

        public async Task<List<Shift>> ImportShift()
        {
            return new List<Shift>();
        }
    }
}
