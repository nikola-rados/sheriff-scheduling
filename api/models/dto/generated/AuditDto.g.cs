namespace SS.Api.models.dto.generated
{
    public partial class AuditDto
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public dynamic KeyValuesJson { get; set; }
        public dynamic OldValuesJson { get; set; }
        public dynamic NewValuesJson { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}