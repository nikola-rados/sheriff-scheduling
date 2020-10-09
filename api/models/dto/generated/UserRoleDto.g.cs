using System;
using SS.Api.Models.Dto;

namespace SS.Api.Models.Dto
{
    public partial class UserRoleDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public RoleDto Role { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}