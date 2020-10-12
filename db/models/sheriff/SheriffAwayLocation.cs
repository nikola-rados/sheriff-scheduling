using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using db.models;
using Mapster;
using SS.Api.Models.DB;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffAwayLocation : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
        public int? LocationId { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public bool IsFullDay { get; set; }
        [AdaptIgnore]
        public virtual Sheriff Sheriff { get; set; }
        public Guid SheriffId { get; set; }
    }
}
