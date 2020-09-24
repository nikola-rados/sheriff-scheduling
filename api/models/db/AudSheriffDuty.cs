using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class AudSheriffDuty
    {
        public string OperationCode { get; set; }
        public DateTime OperationDtm { get; set; }
        public Guid? OperationUserId { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid SheriffDutyId { get; set; }
        public Guid? SheriffId { get; set; }
        public Guid? DutyId { get; set; }
        public DateTime? StartDtm { get; set; }
        public DateTime? EndDtm { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDtm { get; set; }
        public DateTime? UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }
    }
}
