using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthUserRole
    {
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public Guid? LocationId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Location Location { get; set; }
        public virtual AuthRole Role { get; set; }
    }
}
