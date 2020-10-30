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
using SS.Db.models.auth;
using SS.Db.models.lookupcodes;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageTypesController : ControllerBase
    {
        private ManageTypesService ManageTypesService { get; }

        public ManageTypesController(ManageTypesService manageTypesService)
        {
            ManageTypesService = manageTypesService;
        }
        
        [HttpGet]
        [Route("{id}")]
        [PermissionClaimAuthorize(perm: Permission.ViewManageTypes)]
        public async Task<ActionResult<LookupCodeDto>> Find(int id)
        {
            var entity = await ManageTypesService.Find(id);
            if (entity == null)
                return NotFound();
            return Ok(entity.Adapt<LookupCodeDto>());
        }

        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewManageTypes)]
        public async Task<ActionResult<List<LookupCodeDto>>> GetAll(LookupTypes? codeType, int? locationId)
        {
            var lookupCodesDtos = (await ManageTypesService.GetAll(codeType, locationId)).Adapt<List<LookupCodeDto>>();
            return Ok(lookupCodesDtos);
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupCodeDto>> Add(AddLookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null)
                throw new BadRequestException("Invalid lookupCode.");

            var lookupCode = await ManageTypesService.Add(lookupCodeDto);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut("{id}/expire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupCodeDto>> Expire(int id)
        {
            var lookupCode = await ManageTypesService.Expire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut("{id}/unexpire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupCodeDto>> UnExpire(int id)
        {
            var lookupCode = await ManageTypesService.Unexpire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupCodeDto>> Update(LookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null)
                throw new BadRequestException("Invalid lookupCode.");
            var entity = lookupCodeDto.Adapt<LookupCode>();
            var lookupCode = await ManageTypesService.Update(entity);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut("updateSort")]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult> UpdateSortOrders(SortOrdersDto sortOrdersDto)
        {
            if (sortOrdersDto == null)
                throw new BadRequestException("Invalid lookupCode.");
            await ManageTypesService.UpdateSortOrders(sortOrdersDto);
            return NoContent();
        }

    }
}
