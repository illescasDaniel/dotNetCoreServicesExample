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
        [HttpGet]
        public ActionResult<IQueryable<DeviceDto>> Get([FromQuery] int limit = 10)
        {
            var devices = _dbContext.Devices
                            .Take(limit)
                            .ProjectTo<DeviceDto>(_mapper.ConfigurationProvider);
            return Ok(devices);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{id:int}")]
        public ActionResult<DeviceDto> GetById(int id)
        {
            Device? device = _dbContext.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DeviceDto>(device));
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Produces("application/json")]
        //[HttpPatch("{id:int}")]
        //public ActionResult<DeviceDto> PatchById([FromRoute]int id, [FromBody]UpdatedDeviceDto updatedDevice)
        //{
        //    bool hasChanges = false;
        //    Device? device = _dbContext.Devices.Find(id);
        //    if (device == null)
        //    {
        //        return NotFound();
        //    }

        //    if (device.Name != updatedDevice.Name)
        //    {
        //        device.Name = updatedDevice.Name;
        //        hasChanges = true;
        //    }
        //    if (device.Version != updatedDevice.Version)
        //    {
        //        device.Version = updatedDevice.Version;
        //        hasChanges = true;
        //    }

        //    if (!hasChanges)
        //    {
        //        return Ok(updatedDevice);
        //    }

        //    _dbContext.SaveChanges();

        //    var deviceDto = _mapper.Map<DeviceDto>(device);
        //    return Ok(deviceDto);
        //}

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces("application/json")]
        [HttpPatch("{id:int}")]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public ActionResult<DeviceDto> PathById([FromRoute]int id, [FromBody] JsonMergePatchDocument<UpdatedDeviceDto> updatedDevicePatch)
        {
            var hasChanges = false;
            Device? device = _dbContext.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }

            var deviceDto = _mapper.Map<DeviceDto>(device);

            if (updatedDevicePatch.Operations.ToArray().Length == 0)
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

            _dbContext.SaveChanges();
            
            return Ok(deviceDtoAfterChanges);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPut]
        public ActionResult<DeviceDto> Update(DeviceDto updatedDeviceDto)
        {
            bool hasChanges = false;
            Device? device = _dbContext.Devices.Find(updatedDeviceDto.Id);
            if (device == null)
            {
                return NotFound();
            }
            if (_dbContext.Users.Find(updatedDeviceDto.OwnerUserId) == null)
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

            _dbContext.SaveChanges();
            
            var deviceDto = _mapper.Map<DeviceDto>(device);
            return Ok(deviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteById(int id)
        {
            Device? device = _dbContext.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }
            _dbContext.Remove(device);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}