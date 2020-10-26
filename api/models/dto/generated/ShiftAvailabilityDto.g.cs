using System;
using System.Collections.Generic;
using SS.Api.models.dto.generated;
using SS.Db.models.scheduling.notmapped;

namespace SS.Api.models.dto.generated
{
    public partial class ShiftAvailabilityDto
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public List<ShiftConflict> ShiftConflict { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
    }
}