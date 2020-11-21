using System;
using SS.Db.models.sheriff;

namespace SS.Api.models
{
    public class DistributeSchedule
    {
        public Sheriff Sheriff { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string WorkSection { get; set; }
    }
}
