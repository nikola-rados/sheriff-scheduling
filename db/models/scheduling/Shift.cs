using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.Models.DB;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    public class Shift : BaseEntity
    {
        [Key]
        public int Id { get; set;}
        public ShiftType Type { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public ICollection<ShiftSheriff> AssignedSheriffs { get; set; } = new List<ShiftSheriff>();
        public int Slots { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}
