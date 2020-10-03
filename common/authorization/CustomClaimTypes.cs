using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.infrastructure.authorization
{
    public static class CustomClaimTypes
    {
        public const string UserId = nameof(CustomClaimTypes) + nameof(UserId);
        public const string Permission = nameof(CustomClaimTypes)+ nameof(Permission);
        public const string HomeLocationId = nameof(CustomClaimTypes) + nameof(HomeLocationId);
    }
}
