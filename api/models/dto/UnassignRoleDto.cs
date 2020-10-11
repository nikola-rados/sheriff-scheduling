using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.models.dto
{
    public class UnassignRoleDto
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
    }
}
