using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class Leave
    {
        public Guid LeaveId { get; set; }
        public Guid SheriffId { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveSubCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public int PartialDayInd { get; set; }
        public string Comment { get; set; }
        public DateTime? CancelledDtm { get; set; }
        public string LeaveCancelReasonCode { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual LeaveCancelReasonCode LeaveCancelReasonCodeNavigation { get; set; }
        public virtual LeaveSubCode LeaveNavigation { get; set; }
        public virtual Sheriff Sheriff { get; set; }
    }
}
