using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using myMicroservice.Api.v1.Models;
using myMicroservice.Helpers;
using Microsoft.AspNetCore.Identity;
using myMicroservice.Database;
using Microsoft.EntityFrameworkCore;
using myMicroservice.Api.V1.Models;
using System.Collections.Generic;

namespace myMicroservice.Api.v1.Controllers
{
    [ApiController]
    [ApiVersionNeutral]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DeviceController : ControllerBase
    {
        private IUserAuthenticationService _authenticationService;
        private readonly ILogger<UserController> _logger;
        private readonly DatabaseContext _dbContext;

        public DeviceController(
            DatabaseContext dbContext,
            IUserAuthenticationService authenticationService,
            ILogger<UserController> logger
        )
        {
            _dbContext = dbContext;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id:int}")]
        public ActionResult<DeviceDto> GetById(int id)
        {

            Database.Entities.Device? deviceEntity;
            try
            {
                deviceEntity = _dbContext.Devices
                    .AsNoTracking()
                    .FirstOrDefault(d => d.DeviceId == id);
            }
            catch (Exception e)
            {
                return Problem(
                    title: "An internal server error ocurred",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            if (deviceEntity == null)
            {
                return NotFound();
            }

            var user = new DeviceDto(deviceEntity: deviceEntity);
            return Ok(user);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id:int}")]
        public ActionResult<DeviceDto> UpdateById([FromRoute]int id, [FromBody]UpdatedDeviceDto updatedDevice)
        {

            Database.Entities.Device? deviceEntity;
            try
            {
                deviceEntity = _dbContext.Devices
                    .FirstOrDefault(d => d.DeviceId == id);

                if (deviceEntity == null)
                {
                    return NotFound();
                }

                deviceEntity.Name = updatedDevice.Name;
                deviceEntity.Version = updatedDevice.Version;
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return Problem(
                    title: "An internal server error ocurred",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            var user = new DeviceDto(deviceEntity: deviceEntity);
            return Ok(user);
        }
    }
}