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
    public class UserController : ControllerBase
    {
        private IUserAuthenticationService _authenticationService;
        private readonly ILogger<UserController> _logger;
        private readonly DatabaseContext _dbContext;

        public UserController(DatabaseContext dbContext, IUserAuthenticationService authenticationService, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [HttpPost("authenticate")]
        public ActionResult<AuthenticationOutput> Authenticate(AuthenticationModel model)
        {
            Database.Entities.User? userEntity;
            try
            {
                userEntity = _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefault(User => User.Username == model.Username);
            }
            catch (Exception e)
            {
                return Problem(
                    title: "An internal server error ocurred",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            if (userEntity == null)
            {
                return NotFound();
            }

            var passHasher = new PasswordHasher<Database.Entities.User>();
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [HttpPost("register")]
        public ActionResult<UserDto> Register(RegistrationModel model)
        {
            var newUserEntity = model.MapToUserEntity();

            var passHasher = new PasswordHasher<Database.Entities.User>();
            var hashedPass = passHasher.HashPassword(newUserEntity, model.Password);

            newUserEntity.HashedPassword = hashedPass;

            try
            {
                _dbContext.Add(newUserEntity);
                _dbContext.SaveChanges();
                var user = new UserDto(userEntity: newUserEntity);
                return Created($"api/User/{newUserEntity.UserId}", user);
            } catch(DbUpdateException updateException)
            {
                _logger.LogInformation("Tried to insert existing user? ${}");
                _logger.LogInformation(updateException.Message);
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    detail: $"Error adding user. One with the same username might exists"
                );
            } catch (Exception e)
            {
                return Problem(
                    title: "An internal server error ocurred",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id:int}")]
        public ActionResult<UserDto> GetById(int id)
        {

            Database.Entities.User? userEntity;
            try
            {
                userEntity = _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefault(User => User.UserId == id);
            }
            catch (Exception e)
            {
                return Problem(
                    title: "An internal server error ocurred",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            if (userEntity == null)
            {
                return NotFound();
            }

            //_logger.LogInformation(HttpContext.User.Identity.Name); // this is the Name clain we used in jwt (I think)

            var user = new UserDto(userEntity: userEntity);
            return Ok(user);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("{ownerUserId:int}/devices")]
        public ActionResult<DeviceDto> AddDevice([FromBody]NewDeviceDto device, [FromRoute]int ownerUserId)
        {
            try
            {
                var user = _dbContext.Users
                    .Include(User => User.Devices)
                    .FirstOrDefault(User => User.UserId == ownerUserId);

                if (user == null)
                {
                    return NotFound();
                }

                var deviceEntity = device.MapToDeviceEntity();
                user.Devices.Add(deviceEntity);
                _dbContext.SaveChanges();

                var createdDevice = new DeviceDto(deviceEntity: deviceEntity);
                return Created($"api/Device/{deviceEntity.DeviceId}", createdDevice);
            }
            catch (DbUpdateException updateException)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    detail: $"Error adding user device: {updateException.Message}"
                );
            }
            catch (Exception e)
            {
                return Problem(
                    title: "An internal server error ocurred",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // TODO: create device repo ??

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{ownerUserId:int}/devices")]
        public ActionResult<List<DeviceDto>> GetDevices(int ownerUserId)
        {
            try
            {
                var devices = _dbContext.Users
                    .Include(User => User.Devices)
                    .FirstOrDefault(User => User.UserId == ownerUserId)?
                    .Devices;

                if (devices == null)
                {
                    return NotFound();
                }
                return Ok(DeviceDto.DevicesFromEntity(devices));
            }
            catch (DbUpdateException updateException)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    detail: $"Error adding user device: {updateException.Message}"
                );
            }
            catch (Exception e)
            {
                return Problem(
                    title: "An internal server error ocurred",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}