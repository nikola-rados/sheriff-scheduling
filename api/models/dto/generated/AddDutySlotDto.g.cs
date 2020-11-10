using System;

namespace SS.Api.models.dto.generated
{
    public partial class AddDutySlotDto
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public int DutyId { get; set; }
        public Guid? SheriffId { get; set; }
        public int? ShiftId { get; set; }
        public string Timezone { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}