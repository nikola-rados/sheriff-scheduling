using System;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class ActiveRoleWithExpiryDto
    {
        public RoleDto Role { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}