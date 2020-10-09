using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Api.Models.Dto;

namespace SS.Api.models.dto
{
    public class UpdateRoleDto
    {
        public RoleDto Role { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}
