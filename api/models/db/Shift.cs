using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class Shift
    {
        public Guid ShiftId { get; set; }
        public string WorkSectionCode { get; set; }
        public Guid? AssignmentId { get; set; }
        public Guid LocationId { get; set; }
        public Guid? SheriffId { get; set; }
        public DateTime StartDtm { get; set; }
        public DateTime EndDtm { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual Location Location { get; set; }
        public virtual Sheriff Sheriff { get; set; }
        public virtual WorkSectionCode WorkSectionCodeNavigation { get; set; }
    }
}
