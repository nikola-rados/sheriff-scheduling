using System;
using System.Collections.Generic;
using SS.Api.Models.DB;
using SS.Api.Models.Dto;
using SS.Db.models.auth;
using SS.Db.models.scheduling;

namespace SS.Api.Models.Dto
{
    public partial class ShiftDto
    {
        public int Id { get; set; }
        public ShiftType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<SheriffDto> AssignedSheriff { get; set; }
        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? UpdatedById { get; set; }
        public User UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public byte[] RowVersion { get; set; }
    }
}