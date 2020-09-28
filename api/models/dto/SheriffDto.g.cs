using System;
using System.Collections.Generic;
using SS.Api.Models.DB;
using SS.Api.Models.Dto;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.Models.Dto
{
    public partial class SheriffDto
    {
        public Gender Gender { get; set; }
        public string BadgeNumber { get; set; }
        public string Rank { get; set; }
        public List<SheriffAwayLocationDto> AwayLocations { get; set; }
        public List<SheriffLeaveDto> Leaves { get; set; }
        public List<SheriffTrainingDto> Training { get; set; }
        public byte[] Photo { get; set; }
        public Guid Id { get; set; }
        public bool IsDisabled { get; set; }
        public Location HomeLocation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public DateTime? LastLogin { get; set; }
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? UpdatedById { get; set; }
        public User UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public byte[] RowVersion { get; set; }
    }
}