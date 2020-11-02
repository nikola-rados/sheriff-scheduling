using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.models.dto
{
    public class SortOrdersDto
    {
        public int? SortOrderLocationId { get; set; }
        public List<SortOrderDto> SortOrders { get; set; }
    }
}
