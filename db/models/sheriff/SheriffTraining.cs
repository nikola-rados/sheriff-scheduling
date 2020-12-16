using System;
using Mapster;
using ss.db.models;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffTraining : SheriffEvent
    {
        public virtual LookupCode TrainingType { get; set; }
        public int? TrainingTypeId { get; set; }
        public DateTimeOffset? TrainingCertificationExpiry { get; set; }
        public string Note { get; set; }
    }
}
