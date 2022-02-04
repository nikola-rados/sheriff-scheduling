using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SS.Api.controllers.usermanagement.sheriff;
using SS.Api.helpers;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.models.dto;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.controllers.usermanagement
{
    [Route("api/sheriff")]
    [ApiController]
    public class SheriffController : SheriffBaseController
    {
        #region Properties
        // ReSharper disable once InconsistentNaming
        private readonly long _uploadPhotoSizeLimitKB;
        private UserService UserService { get; }
        #endregion

        #region Constructor
        public SheriffController(SheriffService sheriffService, DutyRosterService dutyRosterService, ShiftService shiftService, UserService userService, IConfiguration configuration, SheriffDbContext db) : base (sheriffService, userService, db, shiftService, dutyRosterService)
        {
            _uploadPhotoSizeLimitKB = Convert.ToInt32(configuration.GetNonEmptyValue("UploadPhotoSizeLimitKB"));
            UserService = userService;
        }
        #endregion

        #region Methods 

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateUsers)]
        public async Task<ActionResult<SheriffDto>> AddSheriff(SheriffWithIdirDto addSheriff)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, addSheriff.HomeLocationId)) return Forbid();

            var sheriff = addSheriff.Adapt<Sheriff>();
            sheriff = await SheriffService.AddSheriff(sheriff);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        /// <summary>
        /// This gets a general list of Sheriffs. Includes Training, AwayLocation, Leave data within 7 days.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<SheriffDto>> GetSheriffsForTeams()
        {
            var sheriffs = await SheriffService.GetFilteredSheriffsForTeams();
            return Ok(sheriffs.Adapt<List<SheriffDto>>());
        }

        /// <summary>
        /// This call includes all SheriffAwayLocation, SheriffLeave, SheriffTraining.
        /// </summary>
        /// <param name="id">Guid of the userid.</param>
        /// <returns>SheriffDto</returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [Route("{id}")]
        public async Task<ActionResult<SheriffWithIdirDto>> GetSheriffForTeam(Guid id)
        {
            var sheriff = await SheriffService.GetFilteredSheriffForTeams(id);
            if (sheriff == null) return NotFound(CouldNotFindSheriffError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, sheriff.HomeLocationId)) return Forbid();

            var sheriffDto = sheriff.Adapt<SheriffWithIdirDto>();
            //Prevent exposing Idirs to regular users. 
            sheriffDto.IdirName = User.HasPermission(Permission.EditIdir) ? sheriff.IdirName : null;
            return Ok(sheriffDto);
        }

        /// <summary>
        /// Development route, do not use this in application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [Route("self")]
        public async Task<ActionResult<SheriffDto>> GetSelfSheriff()
        {
            var sheriff = await SheriffService.GetFilteredSheriffForTeams(User.CurrentUserId());
            if (sheriff == null) return NotFound(CouldNotFindSheriffError);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffDto>> UpdateSheriff(SheriffWithIdirDto updateSheriff)
        {
            await CheckForAccessToSheriffByLocation(updateSheriff.Id);

            var canEditIdir = User.HasPermission(Permission.EditIdir);
            var sheriff = updateSheriff.Adapt<Sheriff>();
            sheriff = await SheriffService.UpdateSheriff(sheriff, canEditIdir);

            return Ok(sheriff.Adapt<SheriffDto>());
        }


        [HttpPut]
        [Route("updateLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffDto>> UpdateSheriffHomeLocation(Guid id, int locationId)
        {
            await CheckForAccessToSheriffByLocation(id);

            await SheriffService.UpdateSheriffHomeLocation(id, locationId);
            return NoContent();
        }

        [HttpGet]
        [Route("getPhoto/{id}")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [ResponseCache(Duration = 15552000, Location = ResponseCacheLocation.Client )]
        public async Task<IActionResult> GetPhoto(Guid id) => File(await SheriffService.GetPhoto(id), "image/jpeg");

        [HttpPost]
        [Route("uploadPhoto")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<SheriffDto>> UploadPhoto(Guid? id, string badgeNumber, IFormFile file)
        {
            await CheckForAccessToSheriffByLocation(id, badgeNumber);

            if (file.Length == 0) return BadRequest("File length = 0");
            if (file.Length >= _uploadPhotoSizeLimitKB * 1024) return BadRequest($"File length: {file.Length/1024} KB, Maximum upload size: {_uploadPhotoSizeLimitKB} KB");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            if (!fileBytes.IsImage()) return BadRequest("The uploaded file was not a valid GIF/JPEG/PNG.");

            var sheriff = await SheriffService.UpdateSheriffPhoto(id, badgeNumber, fileBytes);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        #region Roles & Enable User
        [HttpPut]
        [Route("assignRoles")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult> AssignRoles(List<AssignRoleDto> assignRole)
        {
            var entity = assignRole.Adapt<List<UserRole>>();
            await UserService.AssignRolesToUser(entity);
            return NoContent();
        }

        [HttpPut]
        [Route("unassignRoles")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignRoles)]
        public async Task<ActionResult> UnassignRoles(List<UnassignRoleDto> unassignRole)
        {
            var entity = unassignRole.Adapt<List<UserRole>>();
            await UserService.UnassignRoleFromUser(entity);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/enable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireUsers)]
        public async Task<ActionResult<SheriffDto>> EnableUser(Guid id)
        {
            var user = await UserService.EnableUser(id);
            return Ok(user.Adapt<SheriffDto>());
        }

        [HttpPut]
        [Route("{id}/disable")]
        [PermissionClaimAuthorize(perm: Permission.ExpireUsers)]
        public async Task<ActionResult<SheriffDto>> DisableUser(Guid id)
        {
            var user = await UserService.DisableUser(id);
            return Ok(user.Adapt<SheriffDto>());
        }
        #endregion Roles & EnableUser

        #endregion
    }
}
