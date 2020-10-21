using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.controllers
{
    /// <summary>
    /// Used to fetch Locations, plus expire locations. 
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly SheriffDbContext _db;

        public LocationController(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LocationDto>>> Locations()
        {
            var locations = await _db.Location.ApplyPermissionFilters(User,_db).ToListAsync();
            return Ok(locations.Adapt<List<LocationDto>>());
        }

        [HttpGet]
        [Route("all")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LocationDto>>> AllLocations()
        {
            var locations = await _db.Location.ToListAsync();
            return Ok(locations.Adapt<List<LocationDto>>());
        }

        [HttpPut]
        [Route("{id}/enable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireLocation)]
        public async Task<ActionResult> EnableLocation(int id)
        {
            var location = await _db.Location.FindAsync(id);
            location.ThrowBusinessExceptionIfNull($"Couldn't find location with id {id}");
            location.ExpiryDate = null;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/disable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireLocation)]
        public async Task<ActionResult> DisableLocation(int id)
        {
            var location = await _db.Location.FindAsync(id);
            location.ThrowBusinessExceptionIfNull($"Couldn't find location with id {id}");
            location.ExpiryDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
