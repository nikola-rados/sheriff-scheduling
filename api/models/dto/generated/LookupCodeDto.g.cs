using System;
using SS.Api.Models.Dto;
using SS.Db.models.lookupcodes;

namespace SS.Api.Models.Dto
{
    public partial class LookupCodeDto
    {
        public int Id { get; set; }
        public LookupTypes Type { get; set; }
        public string Code { get; set; }
        public string SubCode { get; set; }
        public string Description { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? SortOrder { get; set; }
        public LocationDto Location { get; set; }
        public int? LocationId { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}