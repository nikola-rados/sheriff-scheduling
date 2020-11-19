using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto.generated;
using SS.Api.services.usermanagement;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        public SheriffService SheriffService { get; }
        public SheriffDbContext Db { get; }
        public const string CouldNotFindSheriffError = "Couldn't find sheriff.";

        public AuditController(SheriffService sheriffService, SheriffDbContext db)
        {
            SheriffService = sheriffService;
            Db = db;
        }

        [HttpGet("roleHistory")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult<List<AuditDto>>> ViewRoleHistory(Guid sheriffId)
        {
            var sheriff = await SheriffService.GetSheriff(sheriffId, null);
            if (sheriff == null) return NotFound(CouldNotFindSheriffError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, sheriff.HomeLocationId)) return Forbid();

            var userRoleIds = Db.UserRole.AsNoTracking().Where(ur => ur.UserId == sheriffId).Select(ur => ur.Id);
            var roleHistory = Db.Audit.AsNoTracking().Where(e => e.TableName == "UserRole" &&
                                                  userRoleIds.Contains(e.KeyValues.RootElement.GetProperty("Id")
                                                      .GetInt32()))
                .ToList();

            return Ok(roleHistory.Adapt<List<AuditDto>>());
        }
    }
}
