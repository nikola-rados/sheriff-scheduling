using System;
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
    public class DutySlot : BaseEntity
    {
        [Key, ExcludeFromAddDto]
        public int Id { get; set; }
        public DateTimeOffset StartDate { get; set; }   
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        [ExcludeFromAddAndUpdateDto]
        [AdaptIgnore]
        public Duty Duty { get; set;}
        public int DutyId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Sheriff Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        [AdaptIgnore]
        public Shift Shift { get; set; }
        public int? ShiftId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Location Location { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public int LocationId { get; set; }
        public string Timezone { get; set; }
        public bool IsNotRequired { get; set; }
        public bool IsNotAvailable { get; set; }
        public bool IsOvertime { get; set; }
    }
}
