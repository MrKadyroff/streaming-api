// Controllers/RtmpHooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using StreamApi.Options;

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
    public IActionResult OnPublish([FromForm] RtmpHookRequest req)
    {
        // 1) разберём publicId и query из name / args / tcurl
        var publicId = req.name ?? "";
        var query = req.args ?? "";

        var qIdx = publicId.IndexOf('?', StringComparison.Ordinal);
        if (qIdx >= 0)
        {
            if (string.IsNullOrEmpty(query)) query = publicId[(qIdx + 1)..];
            publicId = publicId[..qIdx];
        }

        if (string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(req.tcurl))
        {
            // tcurl типа rtmp://host/live?key=ABC123
            var uri = new Uri(req.tcurl, UriKind.Absolute);
            query = uri.Query.TrimStart('?');
        }

        if (string.IsNullOrWhiteSpace(publicId)) return Forbid();

        var q = QueryHelpers.ParseQuery(query ?? "");
        var key = q.TryGetValue("key", out var v) ? v.ToString()
                : q.TryGetValue("token", out v) ? v.ToString()
                : null;

        if (string.IsNullOrEmpty(key)) return Forbid();

        // 2) сравнение
        if (_keys.Value.TryGetValue(publicId, out var expected) &&
            string.Equals(expected, key, StringComparison.Ordinal))
            return Ok();

        return Forbid();
    }

    [HttpPost("on_publish_done")]
    public IActionResult OnPublishDone() => Ok();
}
