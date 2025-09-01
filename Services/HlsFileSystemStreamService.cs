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
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            return Enumerable.Empty<StreamInfoDto>();

        var now = DateTimeOffset.UtcNow;
        var threshold = TimeSpan.FromSeconds(Math.Max(1, _opts.ActiveThresholdSeconds));

        var results = new List<StreamInfoDto>();

        // NESTED: /root/<name>/index.m3u8
        foreach (var dir in Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly))
        {
            var idx = Path.Combine(dir, "index.m3u8");
            if (!File.Exists(idx)) continue;

            var name = Path.GetFileName(dir);
            var last = GetNewestTimestamp(dir, idx); // свежий .ts или сам индекс
            var active = (now - last) <= threshold;

            results.Add(new StreamInfoDto(
                Name: name,
                Playlist: $"/hls/{name}/index.m3u8", // предполагается nginx: location /hls { alias /var/www/hls; }
                UpdatedUtc: last,
                Active: active
            ));
        }

        // FLAT: /root/<name>.m3u8
        foreach (var p in Directory.EnumerateFiles(root, "*.m3u8", SearchOption.TopDirectoryOnly))
        {
            // чтобы не дублировать потоки, пропустим index.m3u8 из nested
            if (string.Equals(Path.GetFileName(p), "index.m3u8", StringComparison.OrdinalIgnoreCase))
                continue;

            var name = Path.GetFileNameWithoutExtension(p);
            var dir = Path.GetDirectoryName(p)!;
            var last = GetNewestTimestamp(dir, p);
            var active = (now - last) <= threshold;

            // если этот name уже добавлен из nested — оставим самый свежий
            var existing = results.FirstOrDefault(x => x.Name == name);
            var dto = new StreamInfoDto(name, $"/hls/{name}.m3u8", last, active);

            if (existing is null) results.Add(dto);
            else if (dto.UpdatedUtc > existing.UpdatedUtc)
            {
                results.Remove(existing);
                results.Add(dto);
            }
        }

        return results
            //.Where(s => s.IsActive) // <- если хотите показывать только активные, раскомментируйте
            .OrderByDescending(s => s.UpdatedUtc);

        // локальный хелпер
        static DateTimeOffset GetNewestTimestamp(string folder, string fallbackFile)
        {
            DateTimeOffset newest = new(File.GetLastWriteTimeUtc(fallbackFile), TimeSpan.Zero);
            try
            {
                var tsNewest = Directory.EnumerateFiles(folder, "*.ts", SearchOption.TopDirectoryOnly)
                    .Select(f => new DateTimeOffset(File.GetLastWriteTimeUtc(f), TimeSpan.Zero))
                    .DefaultIfEmpty(newest)
                    .Max();
                if (tsNewest > newest) newest = tsNewest;
            }
            catch { /* ignore */ }
            return newest;
        }
    }

}
