using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        //Todo update location to expire 
    }
}
