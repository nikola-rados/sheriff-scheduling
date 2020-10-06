using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private SheriffDbContext _db;

        public RegionController(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<RegionDto>>> Regions()
        {
            var locations = await _db.Region.ToListAsync();
            return Ok(locations.Adapt<List<RegionDto>>());
        }
    }
}
