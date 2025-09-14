public record CreateAdDto(
    string? Title,
    string? Type,
    string? Position,
    string? ImageUrl,
    string? ClickUrl,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    int? Priority,
    string? TargetAudience
);

public class AdDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? Position { get; set; }
    public string? ImageUrl { get; set; }
    public string? ClickUrl { get; set; }
    public string? Status { get; set; }
    public int Views { get; set; }
    public int Clicks { get; set; }
    public double Ctr { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public int? Priority { get; set; }
    public string? TargetAudience { get; set; }
}
