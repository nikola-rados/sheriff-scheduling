using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class SheriffLocation
    {
        public Guid SheriffLocationId { get; set; }
        public Guid? SheriffId { get; set; }
        public Guid? LocationId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int PartialDayInd { get; set; }
        public decimal? SortOrder { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Location Location { get; set; }
        public virtual Sheriff Sheriff { get; set; }
    }
}
