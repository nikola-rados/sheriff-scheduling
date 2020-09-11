using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class LeaveSubCode
    {
        public LeaveSubCode()
        {
            Leave = new HashSet<Leave>();
        }

        public string LeaveCode { get; set; }
        public string LeaveSubCode1 { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }
        public decimal? SortOrder { get; set; }

        public virtual LeaveCode LeaveCodeNavigation { get; set; }
        public virtual ICollection<Leave> Leave { get; set; }
    }
}
