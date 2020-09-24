using System;
using System.Collections.Generic;

namespace SS.Api.Models.DB
{
    public partial class Location
    {
        public Location()
        {
            Assignment = new HashSet<Assignment>();
            AuthUserRole = new HashSet<AuthUserRole>();
            CourtRoleCode = new HashSet<CourtRoleCode>();
            Courtroom = new HashSet<Courtroom>();
            EscortRun = new HashSet<EscortRun>();
            JailRoleCode = new HashSet<JailRoleCode>();
            OtherAssignCode = new HashSet<OtherAssignCode>();
            SheriffCurrentLocation = new HashSet<Sheriff>();
            SheriffHomeLocation = new HashSet<Sheriff>();
            SheriffLocation = new HashSet<SheriffLocation>();
            Shift = new HashSet<Shift>();
        }

        public Guid LocationId { get; set; }
        public string LocationCd { get; set; }
        public string JustinId { get; set; }
        public string JustinCode { get; set; }
        public string LocationName { get; set; }
        public Guid? ParentLocationId { get; set; }
        public Guid RegionId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<Assignment> Assignment { get; set; }
        public virtual ICollection<AuthUserRole> AuthUserRole { get; set; }
        public virtual ICollection<CourtRoleCode> CourtRoleCode { get; set; }
        public virtual ICollection<Courtroom> Courtroom { get; set; }
        public virtual ICollection<EscortRun> EscortRun { get; set; }
        public virtual ICollection<JailRoleCode> JailRoleCode { get; set; }
        public virtual ICollection<OtherAssignCode> OtherAssignCode { get; set; }
        public virtual ICollection<Sheriff> SheriffCurrentLocation { get; set; }
        public virtual ICollection<Sheriff> SheriffHomeLocation { get; set; }
        public virtual ICollection<SheriffLocation> SheriffLocation { get; set; }
        public virtual ICollection<Shift> Shift { get; set; }
    }
}
