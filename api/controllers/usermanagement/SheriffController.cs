using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SS.Api.Helpers;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Api.models.dto.generated;
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
        private readonly long _uploadPhotoSizeLimitKB;
        public SheriffController(SheriffService service, UserService userService, IConfiguration configuration) : base (userService)
        {
            _service = service;
            _uploadPhotoSizeLimitKB = Convert.ToInt32(configuration.GetNonEmptyValue("UploadPhotoSizeLimitKB"));
        }

        #region Sheriff

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateUsers)]
        public async Task<ActionResult<SheriffDto>> CreateSheriff(CreateSheriffDto sheriffDto)
        {
            var sheriff = sheriffDto.Adapt<Sheriff>();
            sheriff = await _service.CreateSheriff(sheriff);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        /// <summary>
        /// This gets a general list of Sheriffs. Includes Training, AwayLocation, Leave data within 7 days.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(AuthorizeOperation.Or,
            Permission.ViewOwnProfile,
            Permission.ViewProfilesInOwnLocation,
            Permission.ViewProfilesInAllLocation)]
        public async Task<ActionResult<SheriffDto>> GetSheriffs()
        {
            var sheriffs = await _service.GetSheriffs();
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

        /// <summary>
        /// Development route, do not use this in application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(AuthorizeOperation.Or,
            Permission.ViewOwnProfile,
            Permission.ViewProfilesInOwnLocation,
            Permission.ViewProfilesInAllLocation)]
        [Route("self")]
        public async Task<ActionResult<SheriffDto>> GetSelfSheriff()
        {
            var sheriff = await _service.GetSheriff(User.CurrentUserId());
            if (sheriff == null)
                return NotFound($"Couldn't find sheriff.");
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


        [HttpPut]
        [Route("updateLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffDto>> UpdateSheriffHomeLocation(Guid id, int locationId)
        {
            await _service.UpdateSheriffHomeLocation(id, locationId);
            return NoContent();
        }

        [HttpGet]
        [Route("getPhoto/{id}")]
        [PermissionClaimAuthorize(AuthorizeOperation.Or,
            Permission.ViewOwnProfile,
            Permission.ViewProfilesInOwnLocation,
            Permission.ViewProfilesInAllLocation)]
        public async Task<IActionResult> GetPhoto(Guid id)
        {
            return File(await _service.GetPhoto(id), "image/jpeg");
        }

        [HttpPost]
        [Route("uploadPhoto")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffDto>> UploadPhoto(Guid? id, string badgeNumber, IFormFile file)
        {
            if (file.Length == 0)
                return BadRequest("File length = 0");

            if (file.Length >= _uploadPhotoSizeLimitKB * 1024)
                return BadRequest($"File length: {file.Length/1024} KB, Maximum upload size: {_uploadPhotoSizeLimitKB} KB");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            if (!fileBytes.IsImage())
                return BadRequest("The uploaded file was not a valid GIF/JPEG/PNG.");

            var sheriff = await _service.UpdateSheriffPhoto(id, badgeNumber, fileBytes);
            return Ok(sheriff.Adapt<SheriffDto>());
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
        public async Task<ActionResult> RemoveSheriffAwayLocation(int id, string expiryReason)
        {
            await _service.RemoveSheriffAwayLocation(id, expiryReason);
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
        public async Task<ActionResult> RemoveSheriffLeave(int id, string expiryReason)
        {
            await _service.RemoveSheriffLeave(id, expiryReason);
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
        public async Task<ActionResult> RemoveSheriffTraining(int id, string expiryReason)
        {
            await _service.RemoveSheriffTraining(id, expiryReason);
            return NoContent();
        }
        #endregion SheriffTraining

        #region Helpers

 
        #endregion Helpers
    }
}
