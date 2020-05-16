using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using myMicroservice.Models;
using myMicroservice.Helpers;
using Microsoft.AspNetCore.Identity;
using myMicroservice.Database;
using Microsoft.AspNetCore.Authentication;

namespace myMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private IUserAuthenticationService _authenticationService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserAuthenticationService authenticationService, ILogger<UserController> logger)
        {
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
        public ActionResult<AuthenticationOutput> Authenticate(AuthenticationModel model) // todo: change output to Token
        {

            Database.Entities.User? userEntity;
            try
            {
                using (var db = new DatabaseContext())
                {
                    userEntity = db.Users
                     .FirstOrDefault(User => User.Username == model.Username);
                } 
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
        public ActionResult<User> Register(RegistrationModel model)
        {
            var newUserEntity = model.MapToUserEntity();

            var passHasher = new PasswordHasher<Database.Entities.User>();
            var hashedPass = passHasher.HashPassword(newUserEntity, model.Password);

            newUserEntity.HashedPassword = hashedPass;

            try
            {
                using (var db = new DatabaseContext())
                {
                    db.Add(newUserEntity);
                    db.SaveChanges();
                }
                var user = new User(userEntity: newUserEntity);
                return Created($"api/User/{newUserEntity.UserId}", user);
            } catch(Microsoft.EntityFrameworkCore.DbUpdateException updateException)
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
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {

            Database.Entities.User? userEntity;
            try
            {
                using (var db = new DatabaseContext())
                {
                    userEntity = db.Users
                     .FirstOrDefault(User => User.UserId == id);
                }
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

            var user = new User(userEntity: userEntity);
            return Ok(user);
        }
    }
}