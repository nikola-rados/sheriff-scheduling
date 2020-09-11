using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthRolePermission
    {
        public Guid RolePermissionId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? RoleFrontendScopeId { get; set; }
        public Guid? RoleApiScopeId { get; set; }
        public Guid? FrontendScopePermissionId { get; set; }
        public Guid? ApiScopePermissionId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual AuthFrontendScopePermission FrontendScopePermission { get; set; }
        public virtual AuthRole Role { get; set; }
        public virtual AuthRoleApiScope RoleApiScope { get; set; }
        public virtual AuthRoleFrontendScope RoleFrontendScope { get; set; }
    }
}
