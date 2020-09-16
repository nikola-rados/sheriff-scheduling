using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AuthUser
    {
        public Guid UserId { get; set; }
        public string UserAuthId { get; set; }
        public string SiteminderId { get; set; }
        public string DisplayName { get; set; }
        public Guid? SheriffId { get; set; }
        public Guid? DefaultLocationId { get; set; }
        public int SystemAccountInd { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Sheriff Sheriff { get; set; }
        public virtual AuthUser User { get; set; }
        public virtual AuthUser InverseUser { get; set; }
    }
}
