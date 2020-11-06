using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
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
        [ExcludeFromSaveAndAddDto]
        public Duty Duty { get; set;}
        public int DutyId { get; set; }
        [ExcludeFromSaveAndAddDto]
        public Sheriff Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        [ExcludeFromSaveAndAddDto]
        public Shift Shift { get; set; }
        public int? ShiftId { get; set; }
        public string Timezone { get; set; }
    }
}
