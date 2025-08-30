using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamApi.Options;

namespace StreamApi.Controllers;

[ApiController]
[Route("stream")]
public class HooksController : ControllerBase
{
    private readonly StreamApi.Options.PublishKeys _keys;

    public HooksController(IOptions<StreamApi.Options.PublishKeys> keys)
    {
        _keys = keys.Value;
    }

    // NGINX-RTMP on_publish -> POST /stream/validate
    [HttpPost("validate")]
    public async Task<IActionResult> Validate()
    {
        var form = await Request.ReadFormAsync();
        var name = form["name"].ToString();          // имя стрима (/live/<name>)
        var key = Request.Query["key"].ToString();   // ?key=...

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { ok = false, reason = "Missing stream name" });

        if (!_keys.TryGetValue(name, out var expected) || !string.Equals(key, expected, StringComparison.Ordinal))
            return StatusCode(403); // запрет публикации

        return Ok(new { ok = true });
    }

    // NGINX-RTMP on_publish_done -> POST /stream/done
    [HttpPost("done")]
    public async Task<IActionResult> Done()
    {
        var form = await Request.ReadFormAsync();
        var name = form["name"].ToString();
        Console.WriteLine($"Stream finished: {name} @ {DateTimeOffset.UtcNow:o}");
        return Ok(new { ok = true });
    }
}
