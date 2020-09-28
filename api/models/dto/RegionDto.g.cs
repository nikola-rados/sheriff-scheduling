namespace SS.Api.Models.Dto
{
    public partial class RegionDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public byte[] RowVersion { get; set; }
    }
}