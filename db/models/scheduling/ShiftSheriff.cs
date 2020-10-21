using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Db.models.sheriff;

namespace SS.Db.models.scheduling
{
    //Intermediate object for Sheriff <-> Shift.
    [AdaptTo("[name]Dto")]
    public class ShiftSheriff : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Sheriff Sheriff { get; set; }
        public Guid SheriffId { get; set; }
        public Shift Shift { get; set; }
        public int ShiftId { get; set; }
    }
}
