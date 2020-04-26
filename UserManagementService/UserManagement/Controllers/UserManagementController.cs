using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using UserManagement.Domain;
using UserManagement.Model;
using UserManagement.Service;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _service;

        public UserManagementController(IUserManagementService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync([FromBody]UserDto user)
        {
            var result = await _service.RegisterUser(user);
            return Ok(result);
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await _service.Get(id);
            if (result.IsSuccess) return Ok(result.Value);
            else return BadRequest(result.Error);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="pageLimit"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(FilteredResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(int skip = 0, int pageLimit = 0)
        {
            var result = await _service.Get(skip, pageLimit);
            return Ok(result);
        }
    }
}
