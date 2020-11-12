using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Db.models.scheduling;

namespace SS.Api.models
{
    public class ShiftConflict
    {
        public Shift Shift { get; set; }
        public List<string> ConflictMessages { get; set; } = new List<string>();
    }
}
