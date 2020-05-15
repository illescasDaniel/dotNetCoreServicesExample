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

namespace myMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private IAuthenticationService _authenticationService;
        private readonly ILogger<UserController> _logger;

        public UserController(IAuthenticationService authenticationService, ILogger<UserController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<User> Authenticate(AuthenticationModel model)
        {
            // gets a user from input model data
            var user = new User(id: 14, name: model.Username, age: 20);

            _logger.LogInformation($"user {model.Username} will log in");

            return _authenticationService.Authenticate(user);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(string id)
        {
            return Ok(new User(id: 10, "pepe", 22));
        }

        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            //TryValidateModel(user)
            return Ok(user);
        }
    }
}