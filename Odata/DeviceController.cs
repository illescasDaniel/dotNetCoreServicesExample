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
    [ApiVersion("1.0-odata")]
    [ODataRoutePrefix("Devices")]
    // [Authorize(Policy = "ODataServiceApiPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize] // doesn't seem to work!
    public class DeviceController : ODataController
    {
        #region Properties
        private readonly ILogger<DeviceController> _logger;
        private readonly DatabaseContext _dbContext;
        private readonly IUserAuthenticationService _authenticationService;
        #endregion

        #region Initializers
        public DeviceController(DatabaseContext dbContext, ILogger<DeviceController> logger, IUserAuthenticationService authenticationService)
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
        [ProducesResponseType(typeof(ODataValue<ICollection<Device>>), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult Get() // api-version from query
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return Ok(_dbContext.Devices.AsQueryable());
        }

        //[MapToApiVersion( "2.0" )] // if this controller supports v2, this makes this method visible only to v2
        [ODataRoute("{id}")]
        [EnableQuery]
        [ProducesResponseType(typeof(Device), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<Device>> GetById([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Devices
                .AsNoTracking()
                .Where(u => u.DeviceId == id)
            );
        }

        #region User properties

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
                _dbContext.Devices
                .AsNoTracking()
                .Where(u => u.DeviceId == id)
                .Select(u => u.Name)
            );
        }

        [ODataRoute("{id}/Version")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<string>> GetVersion([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Devices
                .AsNoTracking()
                .Where(u => u.DeviceId == id)
                .Select(u => u.Version)
            );
        }

        [ODataRoute("{id}/OwnerUserId")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public ActionResult<SingleResult<int>> GetOwnerUserId([FromODataUri][Required] int id)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            return SingleResult.Create(
                _dbContext.Devices
                .AsNoTracking()
                .Where(u => u.DeviceId == id)
                .Select(u => u.OwnerUserId)
            );
        }

        #endregion

        [ODataRoute]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Device), StatusCodes.Status201Created)]
        [Produces("application/json")]
        public async Task<IActionResult> Create([FromBody] Device device)
        {
            if (!this.IsAuthenticated())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Devices.Add(device);
            await _dbContext.SaveChangesAsync();
            return Created(device);
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
            [FromBody] Delta<Device> delta
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

            var entity = await _dbContext.Devices.FindAsync(id);

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
                if (!DeviceExists(id))
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
            Device? device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            _dbContext.Devices.Remove(device);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        #endregion

        #region Convenience

        bool DeviceExists(int id)
        {
            try
            {
                return _dbContext.Devices.Any(device => device.DeviceId == id);
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}