using System;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Permission.IsAdmin)]
    public class SheriffController : ControllerBase
    {
        private readonly SheriffService _service;
        public SheriffController(SheriffService service)
        {
            _service = service;
        }

        #region Sheriff
        [HttpGet]
        public async Task<ActionResult<SheriffDto>> GetAllSheriffs(bool includeDisabled)
        {
            var sheriffs = await _service.GetSheriffs(includeDisabled);
            return Ok(sheriffs.Adapt<SheriffDto>());
        }

        /// <summary>
        /// This call includes all SheriffAwayLocation, SheriffLeave, SheriffTraining.
        /// </summary>
        /// <param name="id">Guid of the userid.</param>
        /// <returns>SheriffDto</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> FindSheriff(Guid id)
        {
            var sheriff = await _service.GetSheriff(id);
            if (sheriff == null)
                return NotFound($"Couldn't find sheriff with id: {id}");
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSheriff(SheriffDto sheriffDto)
        {
            var sheriff = sheriffDto.Adapt<Sheriff>();
            sheriff = await _service.UpdateSheriff(sheriff);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPut]
        [Route("enableSheriff/{id}")]
        public async Task<ActionResult> EnableSheriff(Guid id)
        {
            var sheriff = await _service.EnableSheriff(id);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpDelete]
        [Route("disableSheriff/{id}")]
        public async Task<ActionResult> DisableSheriff(Guid id)
        {
            var sheriff = await _service.DisableSheriff(id);
            return Ok(sheriff.Adapt<SheriffDto>());
        }
        #endregion Sheriff

        #region SheriffAwayLocation
        [HttpPost]
        [Route("awayLocation")]
        public async Task<ActionResult<SheriffAwayLocationDto>> AddSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto)
        {
            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var createdSheriffAwayLocation = await _service.AddSheriffAwayLocation(sheriffAwayLocation);
            return Ok(createdSheriffAwayLocation);
        }

        [HttpPut]
        [Route("awayLocation")]
        public async Task<ActionResult> UpdateSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto)
        {
            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var updatedSheriffAwayLocation = await _service.UpdateSheriffAwayLocation(sheriffAwayLocation);
            return Ok(updatedSheriffAwayLocation);
        }

        [HttpDelete]
        [Route("awayLocation")]
        public async Task<ActionResult> RemoveSheriffAwayLocation(int id)
        {
            await _service.RemoveSheriffAwayLocation(id);
            return NoContent();
        }
        #endregion

        #region SheriffLeave
        [HttpPost]
        [Route("leave")]
        public async Task<ActionResult<SheriffAwayLocationDto>> AddSheriffLeave(SheriffLeaveDto sheriffLeaveDto)
        {
            var sheriffLeave = sheriffLeaveDto.Adapt<SheriffLeave>();
            var createdSheriffLeave = await _service.AddSheriffLeave(sheriffLeave);
            return Ok(createdSheriffLeave);
        }

        [HttpPut]
        [Route("leave")]
        public async Task<ActionResult> UpdateSheriffAwayLocation(SheriffLeaveDto sheriffAwayLocationDto)
        {
            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var updatedSheriffAwayLocation = await _service.UpdateSheriffAwayLocation(sheriffAwayLocation);
            return Ok(updatedSheriffAwayLocation);
        }

        [HttpDelete]
        [Route("leave")]
        public async Task<ActionResult> RemoveSheriffLeave(int id)
        {
            await _service.RemoveSheriffLeave(id);
            return NoContent();
        }
        #endregion

        #region SheriffTraining
        [HttpPost]
        [Route("training")]
        public async Task<ActionResult<SheriffAwayLocationDto>> AddSheriffTraining(SheriffTrainingDto sheriffTrainingDto)
        {
            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffAwayLocation>();
            var createdSheriffTraining = await _service.AddSheriffAwayLocation(sheriffTraining);
            return Ok(createdSheriffTraining);
        }

        [HttpPut]
        [Route("training")]
        public async Task<ActionResult> UpdateSheriffTraining(SheriffTrainingDto sheriffTrainingDto)
        {
            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffAwayLocation>();
            var updatedSheriffTraining = await _service.UpdateSheriffAwayLocation(sheriffTraining);
            return Ok(updatedSheriffTraining);
        }

        [HttpDelete]
        [Route("training")]
        public async Task<ActionResult> RemoveSheriffTraining(int id)
        {
            await _service.RemoveSheriffTraining(id);
            return NoContent();
        }
        #endregion SheriffTraining
    }
}
