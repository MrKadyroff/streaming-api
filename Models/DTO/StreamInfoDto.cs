namespace StreamApi.Models;

public record StreamInfoDto(
    string Name,
    string Playlist,
    DateTime UpdatedUtc,
    bool Active
);
