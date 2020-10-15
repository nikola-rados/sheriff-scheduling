using System;
using SS.Api.Models.Dto;

namespace SS.Api.Models.Dto
{
    public partial class SheriffAwayLocationDto
    {
        public int Id { get; set; }
        public LocationDto Location { get; set; }
        public int? LocationId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public bool IsFullDay { get; set; }
        public Guid SheriffId { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}