using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;

namespace SS.Api.services.scheduling
{
    public class AssignmentService
    {
        private SheriffDbContext Db { get; }
        public AssignmentService(SheriffDbContext db)
        {
            Db = db;
        }

        public async Task<List<Assignment>> GetAssignments(int locationId)
        {
            //Order by managed types?
            return await Db.Assignment.Where(a => a.LocationId == locationId && a.ExpiryDate == null).ToListAsync();
            //Order by Courtrooms, SubOrder, CourtRoles, SubOrder, JailRoles, SubOrder, EscortRuns, SubOrder, OtherAssignments
        }

        public async Task<Assignment> CreateAssignment(Assignment assignment)
        {
            assignment.Location = await Db.Location.FindAsync(assignment.LocationId);
            assignment.LookupCode = await Db.LookupCode.FindAsync(assignment.LookupCodeId);
            await Db.Assignment.AddAsync(assignment);
            await Db.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAssignment(Assignment entity)
        {
            var savedAssignment = await Db.Assignment.FindAsync(entity.Id);
            savedAssignment.ThrowBusinessExceptionIfNull($"{nameof(Assignment)} with the id: {entity.Id} could not be found.");

            Db.Entry(savedAssignment).CurrentValues.SetValues(entity);

            Db.Entry(savedAssignment).Property(a => a.LocationId).IsModified = false;
            Db.Entry(savedAssignment).Property(a => a.ExpiryDate).IsModified = false;
            Db.Entry(savedAssignment).Property(a => a.ExpiryReason).IsModified = false;

            await Db.SaveChangesAsync();
            return savedAssignment;
        }

        public async Task ExpireAssignment(int id, string expiryReason)
        {
            var savedAssignment = await Db.Assignment.FindAsync(id);
            savedAssignment.ExpiryDate = DateTimeOffset.UtcNow;
            savedAssignment.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }
    }
}
