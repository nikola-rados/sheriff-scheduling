using System;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class AssignmentDto
    {
        public int Id { get; set; }
        public LookupCodeDto LookupCode { get; set; }
        public int LookupCodeId { get; set; }
        public DateTimeOffset? AdhocStartDate { get; set; }
        public DateTimeOffset? AdhocEndDate { get; set; }
        public string Timezone { get; set; }
        public string Name { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public LocationDto Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string ExpiryReason { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}