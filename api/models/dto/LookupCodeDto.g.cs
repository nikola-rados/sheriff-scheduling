using System;
using SS.Api.models.db;
using SS.Api.Models.DB;
using SS.Db.models.auth;

namespace SS.Api.Models.Dto
{
    public partial class LookupCodeDto
    {
        public int Id { get; set; }
        public LookupTypes Type { get; set; }
        public string Code { get; set; }
        public string SubCode { get; set; }
        public string Description { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? SortOrder { get; set; }
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