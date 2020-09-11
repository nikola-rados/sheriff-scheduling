using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthRole
    {
        public AuthRole()
        {
            AuthRoleApiScope = new HashSet<AuthRoleApiScope>();
            AuthRoleFrontendScope = new HashSet<AuthRoleFrontendScope>();
            AuthRolePermission = new HashSet<AuthRolePermission>();
            AuthUserRole = new HashSet<AuthUserRole>();
        }

        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public string Description { get; set; }
        public short? SystemRoleInd { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual ICollection<AuthRoleApiScope> AuthRoleApiScope { get; set; }
        public virtual ICollection<AuthRoleFrontendScope> AuthRoleFrontendScope { get; set; }
        public virtual ICollection<AuthRolePermission> AuthRolePermission { get; set; }
        public virtual ICollection<AuthUserRole> AuthUserRole { get; set; }
    }
}
