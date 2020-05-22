using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using myMicroservice.Helpers;
using myMicroservice.Database;
using myMicroservice.Api.V1.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using myMicroservice.Database.Entities;
using Morcatko.AspNetCore.JsonMergePatch;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace myMicroservice.Api.V1.Controllers
{
    [ApiController]
    [ApiVersionNeutral]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly IUserAuthenticationService _authenticationService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public DeviceController(
            DatabaseContext dbContext,
            IUserAuthenticationService authenticationService,
            ILogger<UserController> logger,
            IMapper mapper
        )
        {
            _dbContext = dbContext;
            _authenticationService = authenticationService;
            _logger = logger;
            _mapper = mapper;
        }

        //

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyCollection<DeviceDto>>> GetAll(
            [FromQuery] int limit = 10
        )
        {
            var devices = await _dbContext.Devices
                            .AsNoTracking()
                            .Take(limit)
                            .ProjectTo<DeviceDto>(_mapper.ConfigurationProvider)
                            .ToListAsync();

            return Ok(devices);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeviceDto>> GetById([FromRoute] int id)
        {
            Device? device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DeviceDto>(device));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces("application/json")]
        [HttpPatch("{id:int}")]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public async Task<ActionResult<DeviceDto>> PatchById(
            [FromRoute]int id,
            [FromBody] JsonMergePatchDocument<UpdatedDeviceDto> updatedDevicePatch
        )
        {
            var hasChanges = false;
            Device? device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            var deviceDto = _mapper.Map<DeviceDto>(device);

            if (updatedDevicePatch.Operations.Count() == 0)
            {
                return Ok(deviceDto);
            }

            var updatedDeviceDto = _mapper.Map<UpdatedDeviceDto>(device);
            updatedDevicePatch.ApplyTo(updatedDeviceDto);

            if (device.Name != updatedDeviceDto.Name)
            {
                device.Name = updatedDeviceDto.Name;
                hasChanges = true;
            }
            if (device.Version != updatedDeviceDto.Version)
            {
                device.Version = updatedDeviceDto.Version;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return Ok(deviceDto);
            }

            var deviceDtoAfterChanges = _mapper.Map<DeviceDto>(device);

            await _dbContext.SaveChangesAsync();
            
            return Ok(deviceDtoAfterChanges);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPut]
        public async Task<ActionResult<DeviceDto>> Update([FromBody] DeviceDto updatedDeviceDto)
        {
            bool hasChanges = false;
            Device? device = await _dbContext.Devices.FindAsync(updatedDeviceDto.Id);
            if (device == null)
            {
                return NotFound();
            }
            User? owner = await _dbContext.Users.FindAsync(updatedDeviceDto.OwnerUserId);
            if (owner == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"Cannot update device because new user Id doesn't exist: {updatedDeviceDto.OwnerUserId}"
                );
            }
            
            if (device.Name != updatedDeviceDto.Name)
            {
                device.Name = updatedDeviceDto.Name;
                hasChanges = true;
            }
            if (device.Version != updatedDeviceDto.Version)
            {
                device.Version = updatedDeviceDto.Version;
                hasChanges = true;
            }
            if (device.OwnerUserId != updatedDeviceDto.OwnerUserId)
            {
                device.OwnerUserId = updatedDeviceDto.OwnerUserId;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return Ok(updatedDeviceDto);
            }

            await _dbContext.SaveChangesAsync();
            
            var deviceDto = _mapper.Map<DeviceDto>(device);
            return Ok(deviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            Device? device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            _dbContext.Remove(device);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}