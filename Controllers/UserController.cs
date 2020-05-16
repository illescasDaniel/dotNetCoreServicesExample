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
using Microsoft.IdentityModel.Tokens;
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
        [Produces("application/json")]
        [HttpPost("authenticate")]
        public ActionResult<User> Authenticate(AuthenticationModel model)
        {
            using var db = new DatabaseContext();

            _logger.LogInformation($"logging user {model.Username}");

            var passHasher = new PasswordHasher<Database.Entities.User>();

            var userEntities = db.Users
                .Where(User => User.Username == model.Username)
                .ToArray();

            if (userEntities.Length == 0)
            {
                return NotFound();
            }

            var userEntity = userEntities.First();

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
                    var user = new User(id: userEntity.UserId, email: userEntity.Email, username: userEntity.Username, token: userToken);
                    return Ok(user);
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
        public ActionResult<User> Register(RegistrationModel model)
        {
            using var db = new DatabaseContext();

            var usersCount = db.Users
                .Where(User => User.Username == model.Username)
                .Count();

            if (usersCount != 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    detail: $"A user with the same username ({model.Username}) already exists"
                );
            }

            var userEntity = new Database.Entities.User(model.Username, model.Password, model.Email);

            var passHasher = new PasswordHasher<Database.Entities.User>();
            var hashedPass = passHasher.HashPassword(userEntity, model.Password);

            userEntity.HashedPassword = hashedPass;

            _logger.LogInformation("Inserting new user");
            db.Add(userEntity);
            db.SaveChanges();

            var user = new User(id: userEntity.UserId, email: userEntity.Email, username: userEntity.Username);
            return Created($"api/User/{userEntity.UserId}", user);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            using var db = new DatabaseContext();

            var userEntities = db.Users
                .Where(User => User.UserId == id)
                .ToArray();

            if (userEntities.Length == 0)
            {
                return NotFound();
            }

            var userEntity = userEntities.First();

            //_logger.LogInformation(HttpContext.User.Identity.Name); // this is the Name clain we used in jwt (I think)

            var user = new User(id: userEntity.UserId, email: userEntity.Email, username: userEntity.Username);
            return Ok(user);
        }
    }
}