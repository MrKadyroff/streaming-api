namespace StreamApi.Models;

public record StreamInfoDto(
    string Name,
    string Playlist,
    DateTimeOffset UpdatedUtc,
    bool Active
);
