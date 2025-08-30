// Controllers/RtmpHooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using StreamApi.Options;

namespace StreamApi.Controllers;

// Use PublishKeys from StreamApi.Options (Dictionary<string,string>)

public record RtmpHookRequest(
    string? call,
    string? app,
    string? name,
    string? addr,
    string? clientid,
    string? flashver,
    string? pageurl,
    string? swfurl,
    string? tcurl,
    string? args // здесь будет key=FOOT2025
);

[ApiController]
[Route("hooks/rtmp")]
public class RtmpHooksController : ControllerBase
{
    private readonly IOptions<PublishKeys> _keys;
    public RtmpHooksController(IOptions<PublishKeys> keys) => _keys = keys;

    [HttpPost("on_publish")]
    public IActionResult OnPublish([FromForm] RtmpHookRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.name))
            return Forbid(); // нет публичного id

        var q = QueryHelpers.ParseQuery(req.args ?? "");
        if (!q.TryGetValue("key", out var keyFromReq))
            return Forbid(); // нет ключа

        if (_keys.Value.TryGetValue(req.name, out var expected) &&
            string.Equals(expected, keyFromReq.ToString(), StringComparison.Ordinal))
        {
            return Ok(); // опубликовать разрешено
        }

        return Forbid(); // неверный ключ
    }

    [HttpPost("on_publish_done")]
    public IActionResult OnPublishDone([FromForm] RtmpHookRequest req)
        => Ok();
}
