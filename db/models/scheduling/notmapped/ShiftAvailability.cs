using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;
using SS.Db.models.sheriff;

namespace SS.Db.models.scheduling.notmapped
{
    [AdaptTo("[name]Dto")]
    public class ShiftAvailability
    {
        [NotMapped]
        public DateTimeOffset StartDate { get; set; }
        [NotMapped]
        public DateTimeOffset EndDate { get; set; }
        [NotMapped]
        public List<ShiftConflict> ShiftConflict { get; set; }
        [NotMapped]
        public Sheriff Sheriff { get; set; }
        [NotMapped]
        public Guid? SheriffId { get; set; }
    }

    public class ShiftConflict
    {
        public DateTimeOffset Date { get; set; }
        public ShiftConflictType Conflict { get; set; }
        public DateTimeOffset ConflictStart { get; set; }
        public DateTimeOffset ConflictEnd { get; set; }
    }

    public enum ShiftConflictType
    {
        Training,
        Leave,
        AwayLocation,
        AlreadyScheduled
    }
}
