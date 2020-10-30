using System.Collections.Generic;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto
{
    public class AddRoleDto
    {
        public RoleDto Role { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
