using System;
using SS.Api.Models.DB;
using SS.Api.Models.Dto;
using SS.Db.models.auth;

namespace SS.Api.Models.Dto
{
    public partial class SheriffAwayLocationDto
    {
        public int Id { get; set; }
        public Location Location { get; set; }
        public int? LocationId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsFullDay { get; set; }
        public Guid? SheriffId { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? UpdatedById { get; set; }
        public User UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public byte[] RowVersion { get; set; }
    }
}