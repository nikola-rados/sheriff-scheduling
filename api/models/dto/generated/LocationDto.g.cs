using System;

namespace SS.Api.Models.Dto
{
    public partial class LocationDto
    {
        public int Id { get; set; }
        public string AgencyId { get; set; }
        public string Name { get; set; }
        public string JustinCode { get; set; }
        public int? ParentLocationId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? RegionId { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}