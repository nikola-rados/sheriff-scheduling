using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthFrontendScopePermission
    {
        public AuthFrontendScopePermission()
        {
            AuthRolePermission = new HashSet<AuthRolePermission>();
        }

        public Guid FrontendScopePermissionId { get; set; }
        public Guid FrontendScopeId { get; set; }
        public string DisplayName { get; set; }
        public string PermissionCode { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual AuthFrontendScope FrontendScope { get; set; }
        public virtual ICollection<AuthRolePermission> AuthRolePermission { get; set; }
    }
}
