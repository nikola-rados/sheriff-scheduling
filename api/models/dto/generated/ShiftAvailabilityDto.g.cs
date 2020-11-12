using System;
using System.Collections.Generic;
using SS.Api.models.dto.generated;
using SS.Db.models.scheduling.notmapped;

namespace SS.Api.models.dto.generated
{
    public partial class ShiftAvailabilityDto
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public List<ShiftAvailabilityConflict> Conflicts { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid? SheriffId { get; set; }
        public string Timezone { get; set; }
    }
}