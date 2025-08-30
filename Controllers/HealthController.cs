using Microsoft.AspNetCore.Mvc;

namespace StreamApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { ok = true, ts = DateTimeOffset.UtcNow });
}
