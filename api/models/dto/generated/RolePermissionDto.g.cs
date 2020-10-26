using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class RolePermissionDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public PermissionDto Permission { get; set; }
        public int PermissionId { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}