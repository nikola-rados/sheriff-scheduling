using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using ss.db.models;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffTraining : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public virtual LookupCode TrainingType { get; set; }
        public int? TrainingTypeId { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public bool IsFullDay { get; set; }
        [AdaptIgnore]
        public virtual Sheriff Sheriff { get; set; }
        public Guid SheriffId { get; set; }
    }
}
