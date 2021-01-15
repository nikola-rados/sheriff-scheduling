using System;

namespace SS.Api.models
{
    public class ShiftBucket
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public string Timezone { get; set; }
    }
}
