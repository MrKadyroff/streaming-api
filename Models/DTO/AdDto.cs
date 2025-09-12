using System;

namespace Models.DTO
{
    public class AdDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Type { get; set; }
        public string? Position { get; set; }
        public string? ImageUrl { get; set; }
        public string? ClickUrl { get; set; }
        public string? Status { get; set; }
        public int? Views { get; set; }
        public int? Clicks { get; set; }
        public double? Ctr { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Priority { get; set; }
        public string? TargetAudience { get; set; }
    }

    public class CreateAdDto
    {
        public string? Title { get; set; }
        public string? Type { get; set; }
        public string? Position { get; set; }
        public string? ImageUrl { get; set; }
        public string? ClickUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Priority { get; set; }
        public string? TargetAudience { get; set; }
    }
}
