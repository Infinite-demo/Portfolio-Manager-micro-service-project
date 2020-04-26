using Microsoft.AspNetCore.Mvc;
using StockManagement.Domain;
using StockManagement.Model;
using StockManagement.Service;
using System.Net;
using System.Threading.Tasks;

namespace StockManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockManagementController : ControllerBase
    {
        private readonly IStockManagementService _service;

        public StockManagementController(IStockManagementService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(StockDetail), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync([FromBody]StockDetailDto user)
        {
            var result = await _service.Add(user);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StockDetail), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await _service.Get(id);
            if (result.IsSuccess) return Ok(result.Value);
            else return BadRequest(result.Error);
        }

        [HttpGet]
        [ProducesResponseType(typeof(FilteredResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(int skip = 0, int pageLimit = 0)
        {
            var result = await _service.Get(skip, pageLimit);
            return Ok(result);
        }
    }
}
