using System;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class SheriffRankDto
    {
        public int Id { get; set; }
        public Guid SheriffId { get; set; }
        public SheriffDto Sheriff { get; set; }
        public string Rank { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}