using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto;
using SS.Api.models.dto.generated;
using SS.Api.services;
using ss.db.models;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.lookupcodes;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageTypesController : ControllerBase
    {
        public const string InvalidLookupCodeError = "Invalid LookupCode.";

        private ManageTypesService ManageTypesService { get; }
        private SheriffDbContext Db { get; }

        public ManageTypesController(ManageTypesService manageTypesService, SheriffDbContext db)
        {
            ManageTypesService = manageTypesService;
            Db = db;
        }
        
        [HttpGet]
        [Route("{id}")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<LookupCodeDto>> Find(int id)
        {
            var entity = await ManageTypesService.Find(id);
            if (entity == null) return NotFound();

            return Ok(entity.Adapt<LookupCodeDto>());
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LookupCodeDto>>> GetAll(LookupTypes? codeType, int? locationId, bool showExpired = false)
        {
            var lookupCodesDtos = (await ManageTypesService.GetAll(codeType, locationId, showExpired)).Adapt<List<LookupCodeDto>>();
            return Ok(lookupCodesDtos);
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupCodeDto>> Add(AddLookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null) return BadRequest(InvalidLookupCodeError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, lookupCodeDto.LocationId)) return Forbid();

            var lookupCode = await ManageTypesService.Add(lookupCodeDto);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut("{id}/expire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupCodeDto>> Expire(int id)
        {
            var entity = await ManageTypesService.Find(id);
            if (entity == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, entity.LocationId)) return Forbid();

            var lookupCode = await ManageTypesService.Expire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut("{id}/unexpire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupCodeDto>> UnExpire(int id)
        {
            var entity = await ManageTypesService.Find(id);
            if (entity == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, entity.LocationId)) return Forbid();

            var lookupCode = await ManageTypesService.Unexpire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupCodeDto>> Update(LookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null) return BadRequest(InvalidLookupCodeError);
            var entity = await ManageTypesService.Find(lookupCodeDto.Id);
            if (entity == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, lookupCodeDto.LocationId)) return Forbid();

            var lookupCode = lookupCodeDto.Adapt<LookupCode>();
            var lookupCodeResult = await ManageTypesService.Update(lookupCode);
            return Ok(lookupCodeResult.Adapt<LookupCodeDto>());
        }

        [HttpPut("updateSort")]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult> UpdateSortOrders(SortOrdersDto sortOrdersDto)
        {
            if (sortOrdersDto == null) return BadRequest(InvalidLookupCodeError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, sortOrdersDto.SortOrderLocationId)) return Forbid();

            await ManageTypesService.UpdateSortOrders(sortOrdersDto);
            return NoContent();
        }

    }
}
