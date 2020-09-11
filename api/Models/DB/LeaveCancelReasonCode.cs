using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class LeaveCancelReasonCode
    {
        public LeaveCancelReasonCode()
        {
            Leave = new HashSet<Leave>();
        }

        public string LeaveCancelReasonCode1 { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual ICollection<Leave> Leave { get; set; }
    }
}
