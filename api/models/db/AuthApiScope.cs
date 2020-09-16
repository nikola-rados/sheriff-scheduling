using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthApiScope
    {
        public AuthApiScope()
        {
            AuthFrontendScopeApi = new HashSet<AuthFrontendScopeApi>();
            AuthRoleApiScope = new HashSet<AuthRoleApiScope>();
        }

        public Guid ApiScopeId { get; set; }
        public string ScopeName { get; set; }
        public string ScopeCode { get; set; }
        public string Description { get; set; }
        public bool SystemScopeInd { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual ICollection<AuthFrontendScopeApi> AuthFrontendScopeApi { get; set; }
        public virtual ICollection<AuthRoleApiScope> AuthRoleApiScope { get; set; }
    }
}
