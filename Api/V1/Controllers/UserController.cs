using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using myMicroservice.Helpers;
using Microsoft.AspNetCore.Identity;
using myMicroservice.Database;
using myMicroservice.Database.Entities;
using Microsoft.EntityFrameworkCore;
using myMicroservice.Api.V1.Models;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace myMicroservice.Api.V1.Controllers
{
    [ApiController]
    [ApiVersionNeutral]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        // DI
        private readonly DatabaseContext _dbContext;
        private readonly IUserAuthenticationService _authenticationService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

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

        //

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost("authenticate")]
        public ActionResult<AuthenticationOutput> Authenticate(AuthenticationModel model)
        {
            User? userEntity;
            userEntity = _dbContext.Users
                .AsNoTracking()
                .FirstOrDefault(User => User.Username == model.Username);

            if (userEntity == null)
            {
                return NotFound();
            }

            var passHasher = new PasswordHasher<User>();
            var verificationResult = passHasher.VerifyHashedPassword(
                userEntity,
                hashedPassword: userEntity.HashedPassword,
                providedPassword: model.Password
            );

            switch (verificationResult)
            {
                case PasswordVerificationResult.Failed:
                    return Unauthorized();
                case PasswordVerificationResult.Success:
                    var userToken = _authenticationService.Authenticate(userEntity.UserId);
                    return Ok(new AuthenticationOutput(userToken, userEntity.UserId));
                case PasswordVerificationResult.SuccessRehashNeeded: // TODO: maybe change to other thing
                    return Problem(
                        title: "Forbidden",
                        detail: $"Correct password, rehash needed (TODO, WIP) for user {model.Username}",
                        statusCode: StatusCodes.Status403Forbidden
                    );
                default:
                    return null!;
            }
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces("application/json")]
        [HttpPost("register")]
        public ActionResult<UserDto> Register(RegistrationModel model)
        {
            var newUserEntity = _mapper.Map<User>(model);

            var passHasher = new PasswordHasher<User>();
            var hashedPass = passHasher.HashPassword(newUserEntity, model.Password);

            newUserEntity.HashedPassword = hashedPass;

            _dbContext.Add(newUserEntity);
            _dbContext.SaveChanges();
            var user = _mapper.Map<UserDto>(newUserEntity);
            return Created($"api/User/{newUserEntity.UserId}", user);
        }

        //

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

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost("{ownerUserId:int}/devices")]
        public ActionResult<DeviceDto> AddDevice([FromBody]NewDeviceDto newDevice, [FromRoute]int ownerUserId)
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

            var createdDeviceDto = _mapper.Map<DeviceDto>(device);//new DeviceDto(device: device);
            return Created($"api/Device/{device.DeviceId}", createdDeviceDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{ownerUserId:int}/devices")]
        public ActionResult<List<DeviceDto>> GetDevices(int ownerUserId)
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

        // patch

        // update

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            User? user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            // automatically deletes devices asociated with it
            _dbContext.Remove(user);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}