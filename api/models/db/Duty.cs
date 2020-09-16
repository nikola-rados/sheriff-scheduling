using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class Duty
    {
        public Duty()
        {
            SheriffDuty = new HashSet<SheriffDuty>();
        }

        public Guid DutyId { get; set; }
        public Guid? DutyRecurrenceId { get; set; }
        public DateTime StartDtm { get; set; }
        public DateTime EndDtm { get; set; }
        public Guid AssignmentId { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual DutyRecurrence DutyRecurrence { get; set; }
        public virtual ICollection<SheriffDuty> SheriffDuty { get; set; }
    }
}
