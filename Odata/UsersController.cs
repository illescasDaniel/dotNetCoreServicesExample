using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using myMicroservice.Database.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using myMicroservice.Helpers;

namespace myMicroservice.Database.Odata
{

    // FIXME: doesn't seem to work except for Get method

    [ApiVersion("1.0-odata")]
    [ODataRoutePrefix("Users")]
    // [Authorize(Policy = "ODataServiceApiPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize] // doesn't seem to work!
    public class UsersController : ODataController
    {
        #region Properties
        private readonly ILogger<UsersController> _logger;
        private readonly DatabaseContext _dbContext;
        private readonly IUserAuthenticationService _authenticationService;
        #endregion

        #region Initializers
        public UsersController(DatabaseContext dbContext, ILogger<UsersController> logger, IUserAuthenticationService authenticationService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _authenticationService = authenticationService;
        }
        #endregion

        #region Actions
        [ODataRoute]
        [EnableQuery(PageSize = 20, MaxTop = 100)] // with paging
        // [EnableQuery( MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count )]
        [
            ProducesResponseType(StatusCodes.Status404NotFound),
            ProducesResponseType(StatusCodes.Status400BadRequest),
            ProducesResponseType(StatusCodes.Status401Unauthorized)
        ]
        [ProducesResponseType(typeof(ODataValue<ICollection<User>>), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult Get() // api-version from query
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return Ok(_dbContext.Users.AsQueryable());
        }

        //[MapToApiVersion( "2.0" )] // if this controller supports v2, this makes this method visible only to v2
        [ODataRoute("{id}")]
        [EnableQuery]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<User>> GetById([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
            );
        }

        #region User properties

        [ODataRoute("{id}/Username")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<string>> GetUsername([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
                .Select(u => u.Username)
            );
        }

        [ODataRoute("{id}/Name")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<string>> GetName([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
                .Select(u => u.Name)
            );
        }

        [ODataRoute("{id}/Surname")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<string>> GetSurname([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
                .Select(u => u.Surname)
            );
        }

        [ODataRoute("{id}/Email")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<string>> GetEmail([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
                .Select(u => u.Email)
            );
        }

        #endregion

        [ODataRoute]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [Produces("application/json")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Created(user);
        }

        [ODataRoute("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces("application/json;odata.metadata=minimal")]
        public async Task<IActionResult> Patch(
            [FromODataUri] int id,
            [FromBody] Delta<User> delta
        )
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _dbContext.Users.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            delta.Patch(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(delta);
        }

        [ODataRoute("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public async Task<IActionResult> Delete([FromODataUri] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            User? user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        #endregion

        #region Convenience

        bool UserExists(int id)
        {
            try
            {
                return _dbContext.Users.Any(user => user.UserId == id);
            } catch(Exception)
            {
                return false;
            }
        }
        #endregion
    }
}