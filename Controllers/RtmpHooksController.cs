using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using StreamApi.Options;

namespace StreamApi.Controllers;

public record RtmpHookRequest(
    string? call, string? app, string? name, string? addr, string? clientid,
    string? flashver, string? pageurl, string? swfurl, string? tcurl, string? args);

[ApiController]
[Route("hooks/rtmp")]
public class RtmpHooksController : ControllerBase
{
    private readonly IOptions<PublishKeys> _keys;  // <— обращаемся к нашему словарю

    public RtmpHooksController(IOptions<PublishKeys> keys) => _keys = keys;

    [HttpPost("on_publish")]
    [AllowAnonymous]
    public IActionResult OnPublish([FromForm] RtmpHookRequest req)
    {
        var publicId = req.name ?? "";
        var query = req.args ?? "";

        // name может прийти как "stream1?key=..."
        var qi = publicId.IndexOf('?', StringComparison.Ordinal);
        if (qi >= 0)
        {
            if (string.IsNullOrEmpty(query)) query = publicId[(qi + 1)..];
            publicId = publicId[..qi];
        }

        // или ключ в tcurl: rtmp://host/live?key=...
        if (string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(req.tcurl))
        {
            var uri = new Uri(req.tcurl, UriKind.Absolute);
            query = uri.Query.TrimStart('?');
        }

        if (string.IsNullOrWhiteSpace(publicId)) return StatusCode(403);

        var q = QueryHelpers.ParseQuery(query ?? "");
        string? key = q.TryGetValue("key", out var v) ? v.ToString()
                  : q.TryGetValue("token", out v) ? v.ToString()
                  : null;

        if (string.IsNullOrEmpty(key)) return StatusCode(403);

        // ТЕПЕРЬ TryGetValue есть у _keys.Value
        if (_keys.Value.TryGetValue(publicId, out var expected) &&
            string.Equals(expected, key, StringComparison.Ordinal))
            return Ok();

        return StatusCode(403);
    }

    [HttpPost("on_publish_done")]
    [AllowAnonymous]
    public IActionResult OnPublishDone([FromForm] RtmpHookRequest _)
        => Ok();
}
