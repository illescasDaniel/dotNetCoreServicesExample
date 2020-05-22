using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using myMicroservice.Helpers;
using myMicroservice.Database;
using myMicroservice.Database.Entities;
using Microsoft.EntityFrameworkCore;
using myMicroservice.Api.V1.Models;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Morcatko.AspNetCore.JsonMergePatch;
using System.Threading.Tasks;
using System;

namespace myMicroservice.Api.V1.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersionNeutral]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        #region Properties
        private readonly DatabaseContext _dbContext;
        private readonly IUserAuthenticationService _authenticationService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        #endregion

        #region Initializers
        public UserController(
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
        #endregion

        #region Actions
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyCollection<UserDto>>> GetAll(
            [FromQuery] int limit = 10
        )
        {
            var users = await _dbContext.Users
                            .AsNoTracking()
                            .Take(limit)
                            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                            .ToListAsync();

            return Ok(users);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById([FromRoute] int id)
        {
            User? user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDto>(user));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{username}")]
        public async Task<ActionResult<UserDto>> GetByUsername(
            [FromRoute] string username
        )
        {
            User? user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Consumes(JsonMergePatchDocument.ContentType)]
        [Produces("application/json")]        
        [HttpPatch("{id:int}")]
        public async Task<ActionResult<UserDto>> PatchById(
            [FromRoute] int id,
            [FromBody] JsonMergePatchDocument<UpdatedUserDto> updatedUserPatch
        )
        {
            var hasChanges = false;
            User? user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (updatedUserPatch.Operations.Count() == 0)
            {
                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }

            var updatedUserDto = _mapper.Map<UpdatedUserDto>(user);
            updatedUserPatch.ApplyTo(updatedUserDto);

            if (user.Name != updatedUserDto.Name)
            {
                user.Name = updatedUserDto.Name;
                hasChanges = true;
            }
            if (user.Surname != updatedUserDto.Surname)
            {
                user.Surname = updatedUserDto.Surname;
                hasChanges = true;
            }
            if (user.Email != updatedUserDto.Email)
            {
                user.Email = updatedUserDto.Email;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }

            var userDtoAfterChanges = _mapper.Map<UserDto>(user);

            await _dbContext.SaveChangesAsync();

            return Ok(userDtoAfterChanges);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPut]
        public async Task<ActionResult<UserDto>> Update(
            [FromBody] UserDto updatedUserDto
        )
        {
            var hasChanges = false;
            User? user = await _dbContext.Users.FindAsync(updatedUserDto.Id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Username != updatedUserDto.Username)
            {
                user.Username = updatedUserDto.Username;
                hasChanges = true;
            }
            if (user.Name != updatedUserDto.Name)
            {
                user.Name = updatedUserDto.Name;
                hasChanges = true;
            }
            if (user.Surname != updatedUserDto.Surname)
            {
                user.Surname = updatedUserDto.Surname;
                hasChanges = true;
            }
            if (user.Email != updatedUserDto.Email)
            {
                user.Email = updatedUserDto.Email;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return Ok(updatedUserDto);
            }

            await _dbContext.SaveChangesAsync();

            var deviceDto = _mapper.Map<UserDto>(user);
            return Ok(deviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById([FromRoute] int id)
        {
            User? user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // automatically deletes devices asociated with it
            // if not, we can use _dbContext.Entry(user).Collections(u => u.Devices).Load() to load devices before deleting them
            _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        #region User devices Actions

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost("{ownerUserId:int}/devices")]
        public async Task<ActionResult<DeviceDto>> AddDevice(
            [FromRoute] int ownerUserId,
            [FromBody] NewDeviceDto newDeviceDto
        )
        {
            var user = await _dbContext.Users
                .Include(User => User.Devices)
                .FirstOrDefaultAsync(User => User.UserId == ownerUserId);

            if (user == null)
            {
                return NotFound();
            }

            var device = _mapper.Map<Device>(newDeviceDto);
            device.OwnerUserId = ownerUserId;

            user.Devices.Add(device);
            await _dbContext.SaveChangesAsync();

            var createdDeviceDto = _mapper.Map<DeviceDto>(device);
            return Created($"api/Device/{device.DeviceId}", createdDeviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{ownerUserId:int}/devices")]
        public async Task<ActionResult<IReadOnlyCollection<DeviceDto>>> GetDevices([FromRoute] int ownerUserId)
        {
            var devices = await _dbContext.Users
                .AsNoTracking()
                .Include(user => user.Devices)
                .Where(user => user.UserId == ownerUserId)
                .Take(1)
                .SelectMany(user => user.Devices)
                .ToListAsync();

            if (devices == null)
            {
                return NotFound();
            }
            var devicesDto = _mapper.Map<List<DeviceDto>>(devices);
            return Ok(devicesDto);
        }
        #endregion
        #endregion
    }
}