using System;
using System.Collections.Generic;
using SS.Api.Models.Dto;
using SS.Db.models.sheriff;

namespace SS.Api.Models.Dto
{
    public partial class SheriffDto
    {
        public Gender Gender { get; set; }
        public string BadgeNumber { get; set; }
        public string Rank { get; set; }
        public List<SheriffAwayLocationDto> AwayLocation { get; set; }
        public List<SheriffLeaveDto> Leave { get; set; }
        public List<SheriffTrainingDto> Training { get; set; }
        public byte[] Photo { get; set; }
        public Guid Id { get; set; }
        public string IdirName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? HomeLocationId { get; set; }
        public LocationDto HomeLocation { get; set; }
        public ICollection<UserRoleDto> UserRoles { get; set; }
        public ICollection<RoleDto> ActiveRoles { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}