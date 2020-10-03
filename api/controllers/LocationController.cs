using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.Helpers.Extensions;
using SS.Api.Models.Dto;
using SS.Db.models;

namespace SS.Api.controllers
{
    /// <summary>
    /// Used to fetch Locations, plus expire locations. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private SheriffDbContext _db;

        public LocationController(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<LocationDto>>> Locations()
        {
            var locations = await _db.Location.ToListAsync();
            return Ok(locations.Adapt<List<LocationDto>>());
        }

        [HttpPut]
        [Route("{id}/enable")]
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
