using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class Sheriff
    {
        public Sheriff()
        {
            AuthUser = new HashSet<AuthUser>();
            Leave = new HashSet<Leave>();
            SheriffDuty = new HashSet<SheriffDuty>();
            SheriffLocation = new HashSet<SheriffLocation>();
            Shift = new HashSet<Shift>();
        }

        public Guid SheriffId { get; set; }
        public string BadgeNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public string SheriffRankCode { get; set; }
        public Guid HomeLocationId { get; set; }
        public Guid? CurrentLocationId { get; set; }
        public string Alias { get; set; }
        public string GenderCode { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Location CurrentLocation { get; set; }
        public virtual GenderCode GenderCodeNavigation { get; set; }
        public virtual Location HomeLocation { get; set; }
        public virtual SheriffRankCode SheriffRankCodeNavigation { get; set; }
        public virtual ICollection<AuthUser> AuthUser { get; set; }
        public virtual ICollection<Leave> Leave { get; set; }
        public virtual ICollection<SheriffDuty> SheriffDuty { get; set; }
        public virtual ICollection<SheriffLocation> SheriffLocation { get; set; }
        public virtual ICollection<Shift> Shift { get; set; }
    }
}
