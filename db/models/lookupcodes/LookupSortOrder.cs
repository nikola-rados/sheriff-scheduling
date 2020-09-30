using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.models.db;
using SS.Api.Models.DB;
using ss.db.models;

namespace SS.Db.models.lookupcodes
{
    public class LookupSortOrder : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public virtual LookupCode LookupCode { get; set; }
        public int? LookupCodeId { get; set; }
        public virtual Location Location { get; set; }
        public int? LocationId { get; set; }
        public LookupTypes LookupType { get; set; }
        public int SortOrder { get; set; }
    }
}
