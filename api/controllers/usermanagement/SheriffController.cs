using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.controllers.usermanagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SheriffController : UserController
    {
        private readonly SheriffService _service;
        public SheriffController(SheriffService service, UserService userService) : base (userService)
        {
            _service = service;
        }

        #region Sheriff

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateUsers)]
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
        [PermissionClaimAuthorize(AuthorizeOperation.Or,
            Permission.ViewOwnProfile,
            Permission.ViewProfilesInOwnLocation,
            Permission.ViewProfilesInAllLocation)]
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
        [PermissionClaimAuthorize(AuthorizeOperation.Or,
            Permission.ViewOwnProfile,
            Permission.ViewProfilesInOwnLocation,
            Permission.ViewProfilesInAllLocation)]
        [Route("{id}")]
        public async Task<ActionResult<SheriffDto>> FindSheriff(Guid id)
        {
            var sheriff = await _service.GetSheriff(id);
            if (sheriff == null)
                return NotFound($"Couldn't find sheriff with id: {id}");
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffDto>> UpdateSheriff(SheriffDto sheriffDto)
        {
            var sheriff = sheriffDto.Adapt<Sheriff>();
            sheriff = await _service.UpdateSheriff(sheriff);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPost]
        [Route("uploadPhoto")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> UploadPhoto(Guid? id, string badgeNumber, byte[] photoData)
        {
            var sheriff = await _service.UpdateSheriffPhoto(id, badgeNumber, photoData);
            return Ok(sheriff);
        }

        #endregion Sheriff

        #region SheriffAwayLocation
        [HttpPost]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffAwayLocationDto>> AddSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto)
        {
            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var createdSheriffAwayLocation = await _service.AddSheriffAwayLocation(sheriffAwayLocation);
            return Ok(createdSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
        }

        [HttpPut]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffAwayLocationDto>> UpdateSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto)
        {
            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<SheriffAwayLocation>();
            var updatedSheriffAwayLocation = await _service.UpdateSheriffAwayLocation(sheriffAwayLocation);
            return Ok(updatedSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
        }

        [HttpDelete]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffAwayLocation(int id)
        {
            await _service.RemoveSheriffAwayLocation(id);
            return NoContent();
        }
        #endregion

        #region SheriffLeave
        [HttpPost]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffLeaveDto>> AddSheriffLeave(SheriffLeaveDto sheriffLeaveDto)
        {
            var sheriffLeave = sheriffLeaveDto.Adapt<SheriffLeave>();
            var createdSheriffLeave = await _service.AddSheriffLeave(sheriffLeave);
            return Ok(createdSheriffLeave.Adapt<SheriffLeaveDto>());
        }

        [HttpPut]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffLeaveDto>> UpdateSheriffLeave(SheriffLeaveDto sheriffLeaveDto)
        {
            var sheriffLeave = sheriffLeaveDto.Adapt<SheriffLeave>();
            var updatedSheriffLeave = await _service.UpdateSheriffLeave(sheriffLeave);
            return Ok(updatedSheriffLeave.Adapt<SheriffLeaveDto>());
        }

        [HttpDelete]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffLeave(int id)
        {
            await _service.RemoveSheriffLeave(id);
            return NoContent();
        }
        #endregion

        #region SheriffTraining
        [HttpPost]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffTrainingDto>> AddSheriffTraining(SheriffTrainingDto sheriffTrainingDto)
        {
            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffTraining>();
            var createdSheriffTraining = await _service.AddSheriffTraining(sheriffTraining);
            return Ok(createdSheriffTraining.Adapt<SheriffTrainingDto>());
        }

        [HttpPut]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffTrainingDto>> UpdateSheriffTraining(SheriffTrainingDto sheriffTrainingDto)
        {
            var sheriffTraining = sheriffTrainingDto.Adapt<SheriffTraining>();
            var updatedSheriffTraining = await _service.UpdateSheriffTraining(sheriffTraining);
            return Ok(updatedSheriffTraining.Adapt<SheriffTrainingDto>());
        }

        [HttpDelete]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffTraining(int id)
        {
            await _service.RemoveSheriffTraining(id);
            return NoContent();
        }
        #endregion SheriffTraining

        #region Helpers

 
        #endregion Helpers
    }
}
