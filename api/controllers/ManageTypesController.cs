using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SS.Api.Helpers.Exceptions;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.Dto;
using SS.Api.services;
using ss.db.models;
using SS.Db.models.auth;
using SS.Db.models.lookupcodes;
using Swashbuckle.AspNetCore.Annotations;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(Role.Administrator, Role.SystemAdministrator)]
    public class ManageTypesController : ControllerBase
    {
        #region Variables

        private readonly ManageTypesService _service;

        #endregion

        public ManageTypesController(ManageTypesService service)
        {
            _service = service;
        }
        
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [Route("{id}")]
        public async Task<ActionResult<LookupCodeDto>> Find(int id)
        {
            var entity = await _service.Find(id);
            if (entity == null)
                return NotFound();
            return Ok(entity.Adapt<LookupCodeDto>());
        }

        [HttpGet]
        public async Task<ActionResult<List<LookupCodeDto>>> GetAll(LookupTypes? codeType, int? locationId)
        {
            var lookupCodesDtos = (await _service.GetAll(codeType, locationId)).Adapt<List<LookupCodeDto>>();
            return Ok(lookupCodesDtos);
        }

        [HttpPost]
        public async Task<ActionResult<LookupCodeDto>> Add(LookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null)
                throw new BadRequestException("Invalid lookupCode.");

            var entity = lookupCodeDto.Adapt<LookupCode>();
            var lookupCode = await _service.Add(entity);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPost("{id}/expire")]
        public async Task<ActionResult<LookupCodeDto>> Expire(int id)
        {
            var lookupCode = await _service.Expire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPost("{id}/unexpire")]
        public async Task<ActionResult<LookupCodeDto>> UnExpire(int id)
        {
            var lookupCode = await _service.Unexpire(id);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpPut]
        public async Task<ActionResult<LookupCodeDto>> Update(LookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null)
                throw new BadRequestException("Invalid lookupCode.");

            var entity = lookupCodeDto.Adapt<LookupCode>();
            var lookupCode = await _service.Update(entity);
            return Ok(lookupCode.Adapt<LookupCodeDto>());
        }

        [HttpDelete]
        public async Task<ActionResult<string>> Remove(int id)
        {
            await _service.Remove(id);
            return NoContent();
        }
    }
}
