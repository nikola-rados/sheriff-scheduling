using System;
using System.Collections.Generic;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class UpdateDutyDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public int? AssignmentId { get; set; }
        public ICollection<UpdateDutySlotDto> DutySlots { get; set; }
        public string Timezone { get; set; }
        public string Comment { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}