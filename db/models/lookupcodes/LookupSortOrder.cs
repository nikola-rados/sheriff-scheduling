using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.Models.DB;
using ss.db.models;

namespace SS.Db.models.lookupcodes
{
    [AdaptTo("[name]Dto")]
    public class LookupSortOrder : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [AdaptIgnore]
        public virtual LookupCode LookupCode { get; set; }
        public int? LookupCodeId { get; set; }
        [AdaptIgnore]
        public virtual Location Location { get; set; }
        public int? LocationId { get; set; }
        public int SortOrder { get; set; }
    }
}
