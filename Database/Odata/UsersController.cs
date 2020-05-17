using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace myMicroservice.Database.Odata
{
    [ApiVersion("1.0")]
    [ODataRoutePrefix("Users")]
    public class UsersController : ODataController
    {

        private readonly ILogger<UsersController> _logger;
        private readonly DatabaseContext _context = new DatabaseContext();

        public UsersController(/*DatabaseContext context, doesn't work? */ILogger<UsersController> logger)
        {
            //_context = context;
            _logger = logger;
        }

        [ODataRoute]
        [EnableQuery]
        public IQueryable<Entities.User> Get()
        {
            return _context.Users;
        }

        [ODataRoute("({id})")]
        [EnableQuery]
        public IQueryable<Entities.User> Get([FromODataUri]int id)
        {
            return _context.Users.Where(u => u.UserId == id);
        }
    }
}