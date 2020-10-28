using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Db.models.sheriff;

namespace SS.Db.models
{
    public class SheriffEvent : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string ExpiryReason { get; set; }
        public Guid SheriffId { get; set; }
        [AdaptIgnore]
        public virtual Sheriff Sheriff { get; set; }
        public string Comment { get; set; }
        public string Timezone { get; set; }
    }
}
