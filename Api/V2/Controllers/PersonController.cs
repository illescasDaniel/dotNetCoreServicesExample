using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myMicroservice.Api.v1.Models;

namespace myMicroservice.Api.v2.Controllers
{
    /// <summary>
    /// GET: api/person
    /// </summary>
    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PersonController : ControllerBase
    {

        // HttpContext.Response looks like that if the current response

        //

        // GET: api/person/{id}
        /// <summary>
        /// A Person by id
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")] // this helps swagger show the correct content-type
        [HttpGet("{id}")]
        public Person GetById(int id)
        {
            return new Person(id: id, name: "juanitooo!!", age: 23);
        }

        // GET: api/person?name="XXXX"
        /// <summary>
        /// Get person by matching name
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [HttpGet]
        public Person GetByName(string name)
        {
            return new Person(id: 15, name: name, age: 23);
        }

        // returning null causes a 204 error
        // https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-3.1#content-negotiation
        //[Produces("application/json")]
        //[HttpPost]
        //public Person? Create(Person person)
        //{
        //    return new Person(id: 100, name: person.Name, age: person.Age);
        //}

        // POST: api/person
        /// <summary>
        /// Creates a new person
        /// </summary>
        /// <param name="person">A person</param>
        /// <returns>Created person or an error</returns>
        /// <remarks>This may fail due to ... bla bla</remarks>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json", "text/plain")]
        [HttpPost]
        public ActionResult<Person> Create(Person person)
        {
            if (person.Name == "Daniel")
            {
                return Problem(
                    title: "Not acceptable",
                    detail: "Cannot create person with name Daniel",
                    statusCode: StatusCodes.Status406NotAcceptable
                );
                // Other options:
                // NoContent
                //return BadRequest("Cannot create person with name Daniel");
                //return new StatusCodeResult(StatusCodes.Status406NotAcceptable);
            }
            return Created("api/person/100", new Person(id: 100, name: person.Name, age: person.Age));
            // also:
            // Ok(...) //200
        }
    }
}
