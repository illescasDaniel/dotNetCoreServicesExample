using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace myMicroservice.Database.Odata
{
    [ApiVersion("1.0-odata")]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ODataValue<IQueryable<Entities.User>>), StatusCodes.Status200OK)]
        [Produces("application/json")]
        // [EnableQuery( MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count )]
        public IQueryable<Entities.User> Get() // api-version from query
        {
            return _dbContext.Users
                .AsNoTracking();
        }

        //[MapToApiVersion( "2.0" )] // if this controller supports v2, this makes this method visible only to v2
        [ODataRoute("({id})")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<Entities.User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public SingleResult<Entities.User> Get([FromODataUri][Required] int id)
        {
            return SingleResult.Create(
                _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
            );
        }

        // User properties

        [ODataRoute("({id})/Username")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<Entities.User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public SingleResult<string> GetUsernameFromUser([FromODataUri][Required] int id)
        {
            return SingleResult.Create(
                _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
                .Select(u => u.Username)
            );
        }
    }
}