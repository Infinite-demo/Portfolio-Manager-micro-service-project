using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace api.gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            var msg = Assembly.GetExecutingAssembly().GetName().Name + " is up & running!";
            return StatusCode(StatusCodes.Status200OK, msg);
        }
    }
}
