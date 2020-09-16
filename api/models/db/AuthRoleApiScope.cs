using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthRoleApiScope
    {
        public AuthRoleApiScope()
        {
            AuthRolePermission = new HashSet<AuthRolePermission>();
        }

        public Guid RoleApiScopeId { get; set; }
        public Guid RoleId { get; set; }
        public Guid ApiScopeId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual AuthApiScope ApiScope { get; set; }
        public virtual AuthRole Role { get; set; }
        public virtual ICollection<AuthRolePermission> AuthRolePermission { get; set; }
    }
}
