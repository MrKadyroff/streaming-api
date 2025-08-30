using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

public class PublishKeys { public Dictionary<string, string> Keys { get; set; } = new(); }

public record RtmpHookRequest(
    string? call, string? app, string? name, string? addr, string? clientid,
    string? flashver, string? pageurl, string? swfurl, string? tcurl, string? args);

[ApiController]
[Route("hooks/rtmp")]
public class RtmpHooksController : ControllerBase
{
    private readonly IOptions<PublishKeys> _keys;
    public RtmpHooksController(IOptions<PublishKeys> keys) => _keys = keys;

    [HttpPost("on_publish")]
    [AllowAnonymous]
    public IActionResult OnPublish([FromForm] RtmpHookRequest req)
    {
        // publicId и query из name?/args/tcurl
        var publicId = req.name ?? "";
        var query = req.args ?? "";

        var i = publicId.IndexOf('?', StringComparison.Ordinal);
        if (i >= 0) { if (string.IsNullOrEmpty(query)) query = publicId[(i + 1)..]; publicId = publicId[..i]; }

        if (string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(req.tcurl))
        {
            var uri = new Uri(req.tcurl, UriKind.Absolute);
            query = uri.Query.TrimStart('?');
        }

        if (string.IsNullOrWhiteSpace(publicId)) return StatusCode(403);

        var q = QueryHelpers.ParseQuery(query ?? "");
        var key = q.TryGetValue("key", out var v) ? v.ToString()
                : q.TryGetValue("token", out v) ? v.ToString()
                : null;

        if (string.IsNullOrEmpty(key)) return StatusCode(403);

        if (_keys.Value.Keys.TryGetValue(publicId, out var expected) &&
            string.Equals(expected, key, StringComparison.Ordinal))
            return Ok();

        return StatusCode(403);
    }

    [HttpPost("on_publish_done")]
    [AllowAnonymous]
    public IActionResult OnPublishDone([FromForm] RtmpHookRequest req) => Ok();
}
