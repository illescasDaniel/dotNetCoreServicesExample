using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using myMicroservice.Helpers;
using Microsoft.AspNetCore.Identity;
using myMicroservice.Database;
using Microsoft.EntityFrameworkCore;
using myMicroservice.Api.V1.Models;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using myMicroservice.Database.Entities;

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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPatch("{id:int}")]
        public ActionResult<DeviceDto> PatchById([FromRoute]int id, [FromBody]UpdatedDeviceDto updatedDevice)
        {
            Device? device = _dbContext.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }
            device.Name = updatedDevice.Name;
            device.Version = updatedDevice.Version;
            _dbContext.Update(device);//Entry(device).State = EntityState.Modified;
            _dbContext.SaveChanges();

            var deviceDto = _mapper.Map<DeviceDto>(device);
            return Ok(deviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPut]
        public ActionResult<DeviceDto> Update(DeviceDto updatedDevice)
        {
            Device? device = _dbContext.Devices.Find(updatedDevice.Id);
            if (device == null)
            {
                return NotFound();
            }
            if (_dbContext.Users.Find(updatedDevice.OwnerUserId) == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"Cannot update device because new user Id doesn't exist: {updatedDevice.OwnerUserId}"
                );
            }

            if (device.Name != updatedDevice.Name)
            {
                device.Name = updatedDevice.Name;
            }
            if (device.Version != updatedDevice.Version)
            {
                device.Version = updatedDevice.Version;
            }
            if (device.OwnerUserId != updatedDevice.OwnerUserId)
            {
                device.OwnerUserId = updatedDevice.OwnerUserId;
            }

            _dbContext.SaveChanges();
            
            var deviceDto = _mapper.Map<DeviceDto>(device);
            return Ok(deviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{id}")]
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