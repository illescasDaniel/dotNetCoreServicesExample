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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            _dbContext.Entry(device).State = EntityState.Modified;

            try
            {
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

            var deviceDto = _mapper.Map<DeviceDto>(device);
            return Ok(deviceDto);
        }
    }
}