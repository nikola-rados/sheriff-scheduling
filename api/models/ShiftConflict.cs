using System.Collections.Generic;
using SS.Db.models.scheduling;

namespace SS.Api.models
{
    public class ShiftConflict
    {
        public Shift Shift { get; set; }
        public List<string> ConflictMessages { get; set; } = new List<string>();
    }
}
