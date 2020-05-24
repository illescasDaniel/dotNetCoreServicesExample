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
using myMicroservice.Api.V2.Models;
using AutoMapper;

namespace myMicroservice.Api.V2.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        // DI
        private readonly DatabaseContext _dbContext;
        private readonly IUserAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IMapper _mapper;

        public AuthenticationController(
            DatabaseContext dbContext,
            IUserAuthenticationService authenticationService,
            ILogger<AuthenticationController> logger,
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
        [Produces("application/x-protobuf")]
        [HttpPost]
        [Route("api/v{version:apiVersion}/authenticate")]
        public ActionResult<AuthenticationOutput> Authenticate(AuthenticationModel model)
        {
            User? userEntity = _dbContext.Users
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
        [Produces("application/x-protobuf")]
        [HttpPost]
        [Route("api/v{version:apiVersion}/register")]
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
    }
}