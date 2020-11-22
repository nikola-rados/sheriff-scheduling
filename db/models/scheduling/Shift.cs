using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using db.models;
using Mapster;
using SS.Api.Models.DB;
using SS.Common.attributes.mapping;
using SS.Db.models.sheriff;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    [GenerateUpdateDto, GenerateAddDto]
    public class Shift : BaseEntity
    {
        [Key, ExcludeFromAddDto]
        public int Id { get; set;}
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Sheriff Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public ICollection<DutySlot> DutySlots { get; set; } = new List<DutySlot>();
        [ExcludeFromAddAndUpdateDto]
        public Assignment AnticipatedAssignment { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
        public bool IsOvertime { get; set; }
        [NotMapped]
        public string WorkSection => DutySlots.FirstOrDefault(ds =>
            ds.StartDate == DutySlots.Min(ds => ds.StartDate))?.AssignmentLookupCode?.Code?.Substring(0,1);
    }
}
