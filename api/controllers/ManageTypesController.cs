using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.Api.Helpers.Exceptions;
using SS.Api.models.db;
using SS.Api.Models.Dto;
using SS.Api.services;
using ss.db.models;
using Swashbuckle.AspNetCore.Annotations;

namespace SS.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageTypesController : ControllerBase
    {
        #region Variables
        ManageTypesService _service;
        #endregion

        public ManageTypesController(ManageTypesService service)
        {
            _service = service;
        }
        
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
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
            var dtos = (await _service.GetAll(codeType, locationId)).Adapt<List<LookupCodeDto>>();
            return Ok(dtos);
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<ActionResult<LookupCodeDto>> Add(LookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null)
                throw new BadRequestException("Invalid lookupCode.");

            var entity = lookupCodeDto.Adapt<LookupCode>();
            var id = await _service.Add(entity);
            return Ok(new LookupCodeDto { Id = id });
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<ActionResult<string>> Update(LookupCodeDto lookupCodeDto)
        {
            if (lookupCodeDto == null)
                throw new BadRequestException("Invalid lookupCode.");

            var entity = lookupCodeDto.Adapt<LookupCode>();

            var success = await _service.Update(entity);
            if (!success)
                return NotFound("Update failed.");
            return Ok("Update successful.");
        }

        [HttpDelete]
        public async Task<ActionResult<string>> Remove(int id)
        {
            await _service.Remove(id);
            return Ok("Remove successful.");
        }
    }
}
