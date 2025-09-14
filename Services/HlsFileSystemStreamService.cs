using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Models.DTO;

namespace Services
{
    public class HlsFileSystemStreamService
    {
        private readonly string _root;
        private readonly int _activeThresholdSeconds;

        public HlsFileSystemStreamService(IOptions<StreamApi.Options.HlsOptions> opts)
        {
            _root = opts.Value.Root;
            _activeThresholdSeconds = opts.Value.ActiveThresholdSeconds;
        }

        public async Task<(IEnumerable<StreamInfoDto> Streams, int Total)> GetAllAsync(int page, int limit, string? status)
        {
            // Синхронная файловая логика, обернутая в Task.Run
            var streams = await Task.Run(() => ListStreams());
            var filtered = streams;
            if (!string.IsNullOrEmpty(status))
                filtered = filtered.Where(s => string.Equals(s.Status, status, StringComparison.OrdinalIgnoreCase));
            var total = filtered.Count();
            var paged = filtered.Skip((page - 1) * limit).Take(limit);
            return (paged, total);
        }

        private IEnumerable<StreamInfoDto> ListStreams()
        {
            Console.WriteLine($"HLS Root: {_root}");
            Console.WriteLine($"Directory exists: {Directory.Exists(_root)}");

            if (string.IsNullOrWhiteSpace(_root) || !Directory.Exists(_root))
                return Enumerable.Empty<StreamInfoDto>();

            var now = DateTimeOffset.UtcNow;
            var threshold = TimeSpan.FromSeconds(Math.Max(1, _activeThresholdSeconds));
            var results = new List<StreamInfoDto>();

            var directories = Directory.EnumerateDirectories(_root, "*", SearchOption.TopDirectoryOnly).ToList();
            Console.WriteLine($"Found directories: {string.Join(", ", directories)}");

            foreach (var dir in directories)
            {
                var idx = Path.Combine(dir, "index.m3u8");
                Console.WriteLine($"Checking: {idx}, exists: {File.Exists(idx)}");

                if (!File.Exists(idx)) continue;
                var name = Path.GetFileName(dir);
                var last = GetNewestTimestamp(dir, idx);
                var active = (now - last) <= threshold;

                Console.WriteLine($"Stream: {name}, Last: {last}, Active: {active}");

                results.Add(new StreamInfoDto
                {
                    Title = name,
                    Status = active ? "live" : "finished",
                    Viewers = 0,
                    StreamUrl = $"/hls/{name}/index.m3u8",
                    FallbackUrl = null,
                    StartTime = last.UtcDateTime,
                    Quality = new List<string> { "auto" }
                });
            }
            return results.OrderByDescending(s => s.StartTime);

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

        public Task<StreamInfoDto?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<StreamInfoDto> CreateAsync(CreateStreamDto dto) => throw new NotImplementedException();
        public Task<bool> UpdateAsync(int id, CreateStreamDto dto) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
        public Task<bool> StartStreamAsync(int id) => throw new NotImplementedException();
        public Task<bool> StopStreamAsync(int id) => throw new NotImplementedException();
    }
}
