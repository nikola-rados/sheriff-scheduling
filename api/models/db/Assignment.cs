using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class Assignment
    {
        public Assignment()
        {
            Duty = new HashSet<Duty>();
            DutyRecurrence = new HashSet<DutyRecurrence>();
            Shift = new HashSet<Shift>();
        }

        public Guid AssignmentId { get; set; }
        public string Title { get; set; }
        public string WorkSectionCode { get; set; }
        public Guid LocationId { get; set; }
        public Guid? CourtroomId { get; set; }
        public Guid? CourtRoleId { get; set; }
        public Guid? JailRoleId { get; set; }
        public Guid? EscortRunId { get; set; }
        public Guid? OtherAssignId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? SortOrder { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual CourtRoleCode CourtRole { get; set; }
        public virtual Courtroom Courtroom { get; set; }
        public virtual EscortRun EscortRun { get; set; }
        public virtual JailRoleCode JailRole { get; set; }
        public virtual Location Location { get; set; }
        public virtual OtherAssignCode OtherAssign { get; set; }
        public virtual ICollection<Duty> Duty { get; set; }
        public virtual ICollection<DutyRecurrence> DutyRecurrence { get; set; }
        public virtual ICollection<Shift> Shift { get; set; }
    }
}
