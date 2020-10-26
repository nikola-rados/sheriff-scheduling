using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.models.dto.generated;
using SS.Db.models;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private SheriffDbContext Db { get; }

        public RegionController(SheriffDbContext dbContext) {  Db = dbContext; }

        [HttpGet]
        public async Task<ActionResult<List<RegionDto>>> Regions()
        {
            var locations = await Db.Region.ToListAsync();
            return Ok(locations.Adapt<List<RegionDto>>());
        }
    }
}
