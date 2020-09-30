using SS.Api.Models.Dto;

namespace SS.Api.Models.Dto
{
    public partial class RolePermissionDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public PermissionDto Permission { get; set; }
        public int PermissionId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}