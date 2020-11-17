using System;

namespace SS.Api.models.dto.generated
{
    public partial class AddShiftDto
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? OriginalEndDate { get; set; }
        public Guid? SheriffId { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}