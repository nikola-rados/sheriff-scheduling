using System.Collections.Generic;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto
{
    public class UpdateRoleDto
    {
        public RoleDto Role { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
