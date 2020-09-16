using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class DutyRecurrence
    {
        public DutyRecurrence()
        {
            Duty = new HashSet<Duty>();
        }

        public Guid DutyRecurrenceId { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public decimal DaysBitmap { get; set; }
        public decimal SheriffsRequired { get; set; }
        public Guid AssignmentId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual ICollection<Duty> Duty { get; set; }
    }
}
