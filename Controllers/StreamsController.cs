
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamApi.Filters;
using StreamApi.Options;
using StreamApi.Services;

namespace StreamApi.Controllers;

[ApiController]
[Route("streams")]
public class StreamsController : ControllerBase
{
    private readonly IStreamService _svc;
    public StreamsController(IStreamService svc) => _svc = svc;

    // требует заголовок Authorization: Bearer <AdminToken>
    [HttpGet]
    public IActionResult List() => Ok(_svc.ListStreams());


    [HttpGet("debug")]
    public IActionResult Debug(
        [FromServices] IOptions<HlsOptions> opts,
        [FromServices] ILogger<StreamsController> log)
    {
        var root = opts.Value.Root;
        var active = opts.Value.ActiveThresholdSeconds;
        var flat = (opts.Value as dynamic)?.FlatLayout; // если есть

        var exists = !string.IsNullOrWhiteSpace(root) && Directory.Exists(root);

        string[] m3u8Top = Array.Empty<string>();
        string[] tsTop = Array.Empty<string>();
        string[] dirsTop = Array.Empty<string>();
        string rootPerms = "";

        try
        {
            if (exists)
            {
                m3u8Top = Directory.EnumerateFiles(root, "*.m3u8", SearchOption.AllDirectories)
                                   .Take(10).Select(p => p.Replace(root, "")).ToArray();
                tsTop = Directory.EnumerateFiles(root, "*.ts", SearchOption.AllDirectories)
                                 .Take(10).Select(p => p.Replace(root, "")).ToArray();
                dirsTop = Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly)
                                   .Take(10).Select(p => p.Replace(root, "")).ToArray();
                var di = new DirectoryInfo(root);
                rootPerms = di.Exists ? di.Attributes.ToString() : "missing";
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Debug listing failed for {Root}", root);
            return Problem($"Listing failed for {root}: {ex.Message}");
        }

        return Ok(new
        {
            HlsRoot = root,
            RootExists = exists,
            ActiveThresholdSeconds = active,
            FlatLayout = flat,
            M3U8Count = exists ? Directory.EnumerateFiles(root, "*.m3u8", SearchOption.AllDirectories).Count() : 0,
            TSCount = exists ? Directory.EnumerateFiles(root, "*.ts", SearchOption.AllDirectories).Count() : 0,
            DirsTop = dirsTop,
            M3u8Top = m3u8Top,
            TsTop = tsTop,
            User = Environment.UserName,
            WorkDir = Environment.CurrentDirectory,
            RootAttrs = rootPerms
        });
    }

}