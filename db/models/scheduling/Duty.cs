using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.Models.DB;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    public class Duty : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public Assignment Assignment { get; set; }
        public int? AssignmentId { get; set; }
        public Shift Shift { get; set; }
        public int ShiftId { get; set; }
        public string Timezone { get; set; }
    }
}
