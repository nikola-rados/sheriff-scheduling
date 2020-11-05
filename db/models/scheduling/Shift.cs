using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using common.attributes;
using db.models;
using Mapster;
using SS.Api.Models.DB;
using SS.Db.models.sheriff;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    [AdaptTo("Add[name]Dto", IgnoreAttributes = new[] { typeof(ExcludeFromAddDtoAttribute) })]
    public class Shift : BaseEntity
    {
        [Key]
        public int Id { get; set;}
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        [ExcludeFromAddDto]
        public Sheriff Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        [ExcludeFromAddDto]
        public ICollection<Duty> Duties { get; set; } = new List<Duty>();
        [ExcludeFromAddDto]
        public Assignment AnticipatedAssignment { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        [ExcludeFromAddDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
    }
}
