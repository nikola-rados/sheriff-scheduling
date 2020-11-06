using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.Models.DB;
using SS.Common.attributes;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    [AdaptTo("Save[name]Dto", IgnoreAttributes = new[] { typeof(ExcludeFromSaveDtoAttribute) })]
    public class Duty : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        [ExcludeFromSaveDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        [ExcludeFromSaveDto]
        public Assignment Assignment { get; set; }
        public int? AssignmentId { get; set; }
        [ExcludeFromSaveDto]
        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
        public string Timezone { get; set; }
    }
}
