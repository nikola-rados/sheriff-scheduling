using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Api.Models.Dto;

namespace SS.Api.models.dto
{
    public class AddRoleDto
    {
        public RoleDto Role { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
