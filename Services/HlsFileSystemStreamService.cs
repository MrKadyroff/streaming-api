using Microsoft.Extensions.Options;
using StreamApi.Models;
using StreamApi.Options;

namespace StreamApi.Services;

public class HlsFileSystemStreamService : IStreamService
{
    private readonly HlsOptions _opts;

    public HlsFileSystemStreamService(IOptions<HlsOptions> opts)
    {
        _opts = opts.Value;
    }

    public IEnumerable<StreamInfoDto> ListStreams()
    {
        var root = _opts.Root;
        if (!Directory.Exists(root))
            return Enumerable.Empty<StreamInfoDto>();

        var threshold = TimeSpan.FromSeconds(Math.Max(1, _opts.ActiveThresholdSeconds));
        var now = DateTimeOffset.UtcNow;

        if (_opts.FlatLayout)
        {
            // /var/www/hls/<name>.m3u8
            var files = Directory.EnumerateFiles(root, "*.m3u8", SearchOption.TopDirectoryOnly);
            return files.Select(f =>
            {
                var name = Path.GetFileNameWithoutExtension(f);
                var info = new FileInfo(f);
                var updated = info.LastWriteTimeUtc;
                var active = (now - updated).TotalSeconds < threshold.TotalSeconds;
                return new StreamInfoDto(name, $"/hls/{name}.m3u8", updated, active);
            }).OrderByDescending(x => x.UpdatedUtc);
        }
        else
        {
            // /var/www/hls/<name>/index.m3u8
            var files = Directory.EnumerateFiles(root, "index.m3u8", SearchOption.AllDirectories);
            return files.Select(idx =>
            {
                var dir = Path.GetDirectoryName(idx)!;
                var name = Path.GetFileName(dir);
                var info = new FileInfo(idx);
                var updated = info.LastWriteTimeUtc;
                var active = (now - updated).TotalSeconds < threshold.TotalSeconds;
                return new StreamInfoDto(name, $"/hls/{name}/index.m3u8", updated, active);
            }).OrderByDescending(x => x.UpdatedUtc);
        }
    }
}
