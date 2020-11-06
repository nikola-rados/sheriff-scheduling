using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.Models.DB;
using SS.Common.attributes;
using SS.Db.models.sheriff;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    [AdaptTo("Save[name]Dto", IgnoreAttributes = new[] { typeof(ExcludeFromSaveDtoAttribute) })]
    public class Shift : BaseEntity
    {
        [Key]
        public int Id { get; set;}
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        [ExcludeFromSaveDto]
        public Sheriff Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        [ExcludeFromSaveDto]
        public ICollection<Duty> Duties { get; set; } = new List<Duty>();
        [ExcludeFromSaveDto]
        public Assignment AnticipatedAssignment { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        [ExcludeFromSaveDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
    }
}
