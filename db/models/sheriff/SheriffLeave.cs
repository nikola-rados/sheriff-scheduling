using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using ss.db.models;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffLeave : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int? LeaveTypeId { get; set; }
        public LookupCode LeaveType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsFullDay { get; set; }
        public Guid? SheriffId { get; set; }
        public Sheriff Sheriff { get; set; }
    }
}
