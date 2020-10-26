using System;
using System.Collections.Generic;
using SS.Api.models.dto.generated;
using SS.Db.models.scheduling;

namespace SS.Api.models.dto.generated
{
    public partial class ShiftDto
    {
        public int Id { get; set; }
        public ShiftType Type { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        public ICollection<DutyDto> Duties { get; set; }
        public AssignmentDto AnticipatedAssignment { get; set; }
        public LocationDto Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}