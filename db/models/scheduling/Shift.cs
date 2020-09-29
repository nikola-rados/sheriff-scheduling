using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.Models.DB;
using SS.Db.models.sheriff;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    public class Shift : BaseEntity
    {
        [Key]
        public int Id { get; set;}
        public ShiftType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual ICollection<Sheriff> AssignedSheriff { get; set; }
        public int? LocationId { get; set; }
        public virtual Location Location { get; set; }
    }
}
