namespace SS.Api.Models.Dto
{
    public partial class LocationDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? JustinId { get; set; }
        public string JustinCode { get; set; }
        public int? ParentLocationId { get; set; }
        public int? RegionId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}