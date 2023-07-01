using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("information")]
        public async Task<IActionResult> GetInformation()
        {
            return Ok("Get Succesfully");
        }
    }
}