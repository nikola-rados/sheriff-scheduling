using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.scheduling;

namespace SS.Api.controllers.scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        public const string InvalidShiftError = "Invalid Shift.";
        public const string CannotUpdateCrossLocationError = "Cannot update cross location.";
        private ShiftService ShiftService { get; }
        private SheriffDbContext Db { get; }

        public ShiftController(ShiftService shiftService, SheriffDbContext db)
        {
            ShiftService = shiftService;
            Db = db;
        }

        #region Shift
        /// <summary>
        /// This is used in the main shift screen, also used in duty roster to populate the available sheriffs on the right hand side. 
        /// </summary>
        [HttpGet]
        [PermissionClaimAuthorize(AuthorizeOperation.Or, Permission.ViewAllShifts, Permission.ViewMyShifts,
            Permission.ViewAllShiftsAtMyLocation)]
        public async Task<ActionResult<List<ShiftDto>>> GetShifts(int locationId, DateTimeOffset start, DateTimeOffset end, bool includeDuties = false)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();
            if (!User.HasPermission(Permission.ViewDuties)) includeDuties = false;

            var shifts = await ShiftService.GetShiftsForLocation(locationId, start, end, includeDuties);
            return Ok(shifts.Adapt<List<ShiftDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public async Task<ActionResult<List<ShiftDto>>> AddShifts(List<AddShiftDto> shiftDtos)
        {
            if (shiftDtos == null) return BadRequest(InvalidShiftError);
            var locationIds = shiftDtos.SelectDistinctToList(s => s.LocationId);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var shift = await ShiftService.AddShifts(shiftDtos.Adapt<List<Shift>>());
            return Ok(shift.Adapt<List<ShiftDto>>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditShifts)]
        public async Task<ActionResult<List<ShiftDto>>> UpdateShifts(List<UpdateShiftDto> shiftDtos)
        {
            if (shiftDtos == null) return BadRequest(InvalidShiftError);
            var locationIds = await ShiftService.GetShiftsLocations(shiftDtos.SelectDistinctToList(s => s.Id));
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var shift = await ShiftService.UpdateShifts(shiftDtos.Adapt<List<Shift>>());
            return Ok(shift.Adapt<List<ShiftDto>>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireShifts)]
        public async Task<ActionResult<ShiftDto>> ExpireShifts(List<int> ids)
        {
            var locationIds = await ShiftService.GetShiftsLocations(ids);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            await ShiftService.ExpireShifts(ids);
            return NoContent();
        }

        [HttpPost]
        [Route("importWeek")]
        [PermissionClaimAuthorize(perm: Permission.ImportShifts)]
        public async Task<ActionResult<ImportedShiftsDto>> ImportWeeklyShifts(int locationId, DateTimeOffset start)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();

            var shifts = await ShiftService.ImportWeeklyShifts(locationId, start);
            return Ok(shifts.Adapt<ImportedShiftsDto>());
        }
        #endregion

        /// <summary>
        /// This is used to show a worker's availability.
        /// </summary>
        [HttpGet]
        [Route("shiftAvailability")]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public async Task<ActionResult<List<ShiftAvailabilityDto>>> GetAvailability(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            if (start >= end) return BadRequest("Start date was on or after end date.");
            if (end.Subtract(start).TotalDays > 30) return BadRequest("End date and start date are more than 30 days apart.");

            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();

            var shiftAvailability = await ShiftService.GetShiftAvailability(start, end, locationId: locationId);
            return Ok(shiftAvailability.Adapt<List<ShiftAvailabilityDto>>());
        }
    }
}
