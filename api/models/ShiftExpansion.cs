using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.models
{
    public class ShiftExpansion
    {
        public Guid SheriffId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Timezone { get; set; }
    }
}
