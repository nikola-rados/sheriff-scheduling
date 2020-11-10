﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;
using SS.Api.Models.DB;
using SS.Db.models.sheriff;

namespace SS.Db.models.scheduling.notmapped
{
    [AdaptTo("[name]Dto")]
    public class ShiftAvailability
    {
        [NotMapped]
        public DateTimeOffset Start { get; set; }
        [NotMapped]
        public DateTimeOffset End { get; set; }
        [NotMapped]
        public List<ShiftConflict> Conflicts { get; set; }
        [NotMapped]
        public Sheriff Sheriff { get; set; }
        [NotMapped]
        public Guid? SheriffId { get; set; }
        [NotMapped]
        public string Timezone { get; set; }
    }

    public class ShiftConflict
    {
        public Guid? SheriffId { get; set; }
        public ShiftConflictType Conflict { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public int? LocationId { get; set;}
        public Location Location { get; set; }
        public int? ShiftId { get; set; }
    }

    public enum ShiftConflictType
    {
        Training,
        Leave,
        AwayLocation,
        Scheduled
    }
}