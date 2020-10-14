using System;
using SS.Api.Models.Dto;

namespace SS.Api.Models.Dto
{
    public partial class ActiveRoleWithExpiryDto
    {
        public RoleDto Role { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}