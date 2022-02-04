using System;

namespace SS.Api.models.dto.generated
{
    public partial class UpdateSheriffRankDto
    {
        public int Id { get; set; }
        public Guid SheriffId { get; set; }
        public string Rank { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}