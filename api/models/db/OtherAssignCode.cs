using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class OtherAssignCode
    {
        public OtherAssignCode()
        {
            Assignment = new HashSet<Assignment>();
        }

        public Guid OtherAssignId { get; set; }
        public Guid? LocationId { get; set; }
        public string OtherAssignCode1 { get; set; }
        public string OtherAssignName { get; set; }
        public string Description { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? SortOrder { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Location Location { get; set; }
        public virtual ICollection<Assignment> Assignment { get; set; }
    }
}
