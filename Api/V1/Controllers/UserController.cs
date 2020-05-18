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

namespace myMicroservice.Api.V1.Controllers
{
    [ApiController]
    [ApiVersionNeutral]
    [Authorize]
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
        [HttpGet]
        public ActionResult<IQueryable<UserDto>> Get([FromQuery] int limit = 10)
        {
            var users = _dbContext.Users
                            .Take(limit)
                            .ProjectTo<UserDto>(_mapper.ConfigurationProvider);
            return Ok(users);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{id:int}")]
        public ActionResult<UserDto> GetById(int id)
        {
            User? user = _dbContext.Users.Find(id);
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
        public ActionResult<UserDto> GetByUsername([FromRoute]string username)
        {
            User? user = _dbContext.Users
                .Where(u => u.Username == username)
                .FirstOrDefault();
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
        [Produces("application/json")]
        [HttpPatch("{id:int}")]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public ActionResult<UserDto> PathById([FromRoute]int id, [FromBody] JsonMergePatchDocument<UpdatedUserDto> updatedUserPatch)
        {
            var hasChanges = false;
            User? user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);

            if (updatedUserPatch.Operations.ToArray().Length == 0)
            {
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
                return Ok(userDto);
            }

            var userDtoAfterChanges = _mapper.Map<UserDto>(user);

            _dbContext.SaveChanges();

            return Ok(userDtoAfterChanges);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces("application/json")]
        [HttpPut]
        public ActionResult<UserDto> Update(UserDto updatedUserDto)
        {
            var hasChanges = false;
            User? user = _dbContext.Users.Find(updatedUserDto.Id);
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

            _dbContext.SaveChanges();

            var deviceDto = _mapper.Map<UserDto>(user);
            return Ok(deviceDto);
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //[Produces("application/json")]
        //[HttpPut]
        //public ActionResult<UserDto> Update(UserDto updatedUserDto)
        //{
        //    var hasChanges = false;
        //    User? user = _dbContext.Users.Find(updatedUserDto.Id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    if (user.Username != updatedUserDto.Username)
        //    {
        //        user.Username = updatedUserDto.Username;
        //        hasChanges = true;
        //    }
        //    if (user.Name != updatedUserDto.Name)
        //    {
        //        user.Name = updatedUserDto.Name;
        //        hasChanges = true;
        //    }
        //    if (user.Surname != updatedUserDto.Surname)
        //    {
        //        user.Surname = updatedUserDto.Surname;
        //        hasChanges = true;
        //    }
        //    if (user.Email != updatedUserDto.Email)
        //    {
        //        user.Email = updatedUserDto.Email;
        //        hasChanges = true;
        //    }

        //    if (!hasChanges)
        //    {
        //        return Ok(updatedUserDto);
        //    }

        //    _dbContext.SaveChanges();

        //    var deviceDto = _mapper.Map<UserDto>(user);
        //    return Ok(deviceDto);
        //}

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteById(int id)
        {
            User? user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            // automatically deletes devices asociated with it
            // if not, we can use _dbContext.Entry(user).Collections(u => u.Devices).Load() to load devices before deleting them
            _dbContext.Remove(user);
            _dbContext.SaveChanges();

            return Ok();
        }

        #region User devices Actions

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost("{ownerUserId:int}/devices")]
        public ActionResult<DeviceDto> AddDevice([FromRoute]int ownerUserId, [FromBody]NewDeviceDto newDevice)
        {
            var user = _dbContext.Users
                .Include(User => User.Devices)
                .FirstOrDefault(User => User.UserId == ownerUserId);

            if (user == null)
            {
                return NotFound();
            }

            var device = newDevice.MapToDeviceEntity();
            user.Devices.Add(device);
            _dbContext.SaveChanges();

            var createdDeviceDto = _mapper.Map<DeviceDto>(device);
            return Created($"api/Device/{device.DeviceId}", createdDeviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{ownerUserId:int}/devices")]
        public ActionResult<ICollection<DeviceDto>> GetDevices(int ownerUserId)
        {
            var devices = _dbContext.Users
                .Include(User => User.Devices)
                .FirstOrDefault(User => User.UserId == ownerUserId)?
                .Devices;

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