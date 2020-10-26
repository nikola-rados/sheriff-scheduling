using System;
using SS.Api.models.dto.generated;
using SS.Db.models.lookupcodes;

namespace SS.Api.models.dto.generated
{
    public partial class LookupCodeDto
    {
        public int Id { get; set; }
        public LookupTypes Type { get; set; }
        public string Code { get; set; }
        public string SubCode { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public int? SortOrder { get; set; }
        public LocationDto Location { get; set; }
        public int? LocationId { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}