using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthRoleFrontendScope
    {
        public AuthRoleFrontendScope()
        {
            AuthRolePermission = new HashSet<AuthRolePermission>();
        }

        public Guid RoleFrontendScopeId { get; set; }
        public Guid RoleId { get; set; }
        public Guid FrontendScopeId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual AuthFrontendScope FrontendScope { get; set; }
        public virtual AuthRole Role { get; set; }
        public virtual ICollection<AuthRolePermission> AuthRolePermission { get; set; }
    }
}
