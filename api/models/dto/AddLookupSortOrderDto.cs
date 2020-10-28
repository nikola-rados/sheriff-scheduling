using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.models.dto
{
    public class AddLookupSortOrderDto
    {
        public int? LocationId { get; set; }
        public int SortOrder { get; set; }
    }
}
