using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.controllers.usermanagement
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(Role.Administrator, Role.SystemAdministrator)]
    public class SheriffController : UserController
    {
        private readonly SheriffService _service;
        public SheriffController(SheriffService service, UserService userService) : base (userService)
        {
            _service = service;
        }

        #region Sheriff

        //This uses Sheriff because it's an extended object of User. 
        [AuthorizeRoles(Role.SystemAdministrator)]
        [HttpPost]
        public async Task<ActionResult<SheriffDto>> CreateSheriff(SheriffDto sheriffDto)
        {
            var sheriff = sheriffDto.Adapt<Sheriff>();
            sheriff = await _service.CreateSheriff(sheriff);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        /// <summary>
        /// This gets a general list of Sheriffs, without all the details. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<SheriffDto>> GetSheriffs(int? locationId)
        {
            var sheriffs = await _service.GetSheriffs(locationId);
            return Ok(sheriffs.Adapt<List<SheriffDto>>());
        }

        /// <summary>
        /// This call includes all SheriffAwayLocation, SheriffLeave, SheriffTraining.
        /// </summary>
        /// <param name="id">Guid of the userid.</param>
        /// <returns>SheriffDto</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<SheriffDto>> FindSheriff(Guid id)
        {
            var sheriff = await _service.GetSheriff(id);
            if (sheriff == null)
                return NotFound($"Couldn't find sheriff with id: {id}");
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPut]
        public async Task<ActionResult<SheriffDto>> UpdateSheriff(SheriffDto sheriffDto)
        {
            var sheriff = sheriffDto.Adapt<Sheriff>();
            sheriff = await _service.UpdateSheriff(sheriff);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPost]
        [Route("uploadPhoto")]
        public async Task<ActionResult> UploadPhoto(Guid? id, string badgeNumber, IFormFile file)
        {
            if (file.Length == 0)
                return BadRequest("File length = 0");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var sheriff = await _service.UpdateSheriffPhoto(id, badgeNumber, fileBytes);
            return Ok(sheriff);
        }

        #endregion Sheriff

        #region SheriffAwayLocation
        [HttpPost]
        [Route("awayLocation")]
        public async Task<ActionResult<SheriffAwayLocationDto>> AddSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto)
        {
            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var createdSheriffAwayLocation = await _service.AddSheriffAwayLocation(sheriffAwayLocation);
            return Ok(createdSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
        }

        [HttpPut]
        [Route("awayLocation")]
        public async Task<ActionResult<SheriffAwayLocationDto>> UpdateSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto)
        {
            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var updatedSheriffAwayLocation = await _service.UpdateSheriffAwayLocation(sheriffAwayLocation);
            return Ok(updatedSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
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
        public async Task<ActionResult<SheriffLeaveDto>> AddSheriffLeave(SheriffLeaveDto sheriffLeaveDto)
        {
            var sheriffLeave = sheriffLeaveDto.Adapt<SheriffLeave>();
            var createdSheriffLeave = await _service.AddSheriffLeave(sheriffLeave);
            return Ok(createdSheriffLeave.Adapt<SheriffLeaveDto>());
        }

        [HttpPut]
        [Route("leave")]
        public async Task<ActionResult<SheriffLeaveDto>> UpdateSheriffLeave(SheriffLeaveDto sheriffLeaveDto)
        {
            var sheriffLeave = sheriffLeaveDto.Adapt<SheriffLeave>();
            var updatedSheriffLeave = await _service.UpdateSheriffLeave(sheriffLeave);
            return Ok(updatedSheriffLeave.Adapt<SheriffLeaveDto>());
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
        public async Task<ActionResult<SheriffTrainingDto>> AddSheriffTraining(SheriffTrainingDto sheriffTrainingDto)
        {
            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffTraining>();
            var createdSheriffTraining = await _service.AddSheriffTraining(sheriffTraining);
            return Ok(createdSheriffTraining.Adapt<SheriffTrainingDto>());
        }

        [HttpPut]
        [Route("training")]
        public async Task<ActionResult<SheriffTrainingDto>> UpdateSheriffTraining(SheriffTrainingDto sheriffTrainingDto)
        {
            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffTraining>();
            var updatedSheriffTraining = await _service.UpdateSheriffTraining(sheriffTraining);
            return Ok(updatedSheriffTraining.Adapt<SheriffTrainingDto>());
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
