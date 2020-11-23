using System;
using System.Collections.Generic;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class ShiftDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        public ICollection<DutySlotDto> DutySlots { get; set; }
        public AssignmentDto AnticipatedAssignment { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        public LocationDto Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
        public bool IsOvertime { get; set; }
        public string WorkSection { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}