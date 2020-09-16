using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthFrontendScopeApi
    {
        public Guid FrontendScopeApiId { get; set; }
        public Guid FrontendScopeId { get; set; }
        public Guid ApiScopeId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual AuthApiScope ApiScope { get; set; }
        public virtual AuthFrontendScope FrontendScope { get; set; }
    }
}
