using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public DateTimeOffset? OriginalEndDate { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Sheriff Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public ICollection<Duty> Duties { get; set; } = new List<Duty>();
        [ExcludeFromAddAndUpdateDto]
        public Assignment AnticipatedAssignment { get; set; }
        public int? AnticipatedAssignmentId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Timezone { get; set; }
    }
}
