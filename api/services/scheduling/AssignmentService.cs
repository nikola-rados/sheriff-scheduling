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

        public async Task<List<Assignment>> GetAssignments(int locationId, DateTimeOffset? start, DateTimeOffset? end)
        {
            //Order by LookupCodeType, SortOrder, then lack of SortOrder. 
            var assignment = await Db.Assignment.AsSplitQuery().AsNoTracking()
                .Include(a => a.Location)
                .Include(a => a.LookupCode)
                .ThenInclude(lc => lc.SortOrder.Where(so => so.LocationId == locationId))
                .Where(a => a.LocationId == locationId && a.ExpiryDate == null)
                .OrderBy(a => (int)a.LookupCode.Type)
                .ThenBy(a => a.LookupCode.SortOrder.First().SortOrder)
                .ThenBy(a => !a.LookupCode.SortOrder.Any())
                .ToListAsync();

            //Filter out the date ranges outside of the database. 
            var filteredAssignments = assignment.WhereToList(a => a.HasAtLeastOneDayOverlap(start, end));
            filteredAssignments.ForEach(lc => lc.LookupCode.SortOrderForLocation = lc.LookupCode.SortOrder.FirstOrDefault());
            return filteredAssignments;
        }

        public async Task<Assignment> CreateAssignment(Assignment assignment)
        {
            assignment.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(assignment.Timezone)} needs to be included in the assignment.");
            assignment.Location = await Db.Location.FindAsync(assignment.LocationId);
            assignment.LookupCode = await Db.LookupCode.FindAsync(assignment.LookupCodeId);
            await Db.Assignment.AddAsync(assignment);
            await Db.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAssignment(Assignment assignment)
        {
            assignment.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(assignment.Timezone)} needs to be included in the assignment.");
            var savedAssignment = await Db.Assignment.FindAsync(assignment.Id);
            savedAssignment.ThrowBusinessExceptionIfNull($"{nameof(Assignment)} with the id: {assignment.Id} could not be found.");

            Db.Entry(savedAssignment).CurrentValues.SetValues(assignment);
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
