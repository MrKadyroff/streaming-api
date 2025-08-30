
namespace StreamApi.Options;

public class HlsOptions
{
    public string Root { get; set; } = "/var/www/hls";
    public bool FlatLayout { get; set; } = true; // true: /hls/<name>.m3u8 ; false: /hls/<name>/index.m3u8
    public int ActiveThresholdSeconds { get; set; } = 15;
}
