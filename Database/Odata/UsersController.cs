using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Database.Odata
{
    [ApiVersion("1.0")]
    [ODataRoutePrefix("Users")]
    [Authorize]
    public class UsersController : ODataController
    {
        private readonly ILogger<UsersController> _logger;
        private readonly DatabaseContext _dbContext;

        public UsersController(DatabaseContext dbContext, ILogger<UsersController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [ODataRoute]
        [EnableQuery]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<Entities.UserEntity>>), StatusCodes.Status200OK)]
        [Produces("application/json")]
        // [EnableQuery( MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count )]
        public IQueryable<Entities.UserEntity> Get() // api-version from query
        {
            return _dbContext.Users;
        }

        //[MapToApiVersion( "2.0" )] // if this controller supports v2, this makes this method visible only to v2
        [ODataRoute("({id})")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<Entities.UserEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public ActionResult<Entities.UserEntity> Get([FromODataUri][Required] int id)
        {
            var user = _dbContext.Users.Where(u => u.UserId == id).FirstOrDefault();
            if (user == null)
            {
                return Problem(
                    title: "Not found",
                    statusCode: StatusCodes.Status204NoContent
                );
            }
            return Ok(user);
        }
    }
}