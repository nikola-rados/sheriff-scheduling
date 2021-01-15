using System.Collections.Generic;

namespace SS.Api.models.dto
{
    public class SortOrdersDto
    {
        public int? SortOrderLocationId { get; set; }
        public List<SortOrderDto> SortOrders { get; set; }
    }
}
