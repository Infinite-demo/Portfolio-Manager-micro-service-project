using Microsoft.AspNetCore.Mvc;
using StockReport.Domain;
using StockReport.Model;
using StockReport.Service;
using System.Net;
using System.Threading.Tasks;

namespace StockReport.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockReportController : ControllerBase
    {
        private readonly IStockReportService _service;

        public StockReportController(IStockReportService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(FilteredResult<StockReportDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(int skip = 0, int pageLimit = 0)
        {
            var result = await _service.Get(skip, pageLimit);
            return Ok(result);
        }
    }
}
