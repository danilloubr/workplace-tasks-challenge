using Microsoft.AspNetCore.Mvc;

namespace WorkplaceTasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        // GET: /api/health
        [HttpGet]
        public IActionResult CheckHealth()
        {
            return Ok(new { Status = "Healthy" });
        }
    }
}