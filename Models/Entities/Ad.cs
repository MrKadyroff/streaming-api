using System;

namespace Models.Entities
{
    public class Ad
    {
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Type { get; set; }            // banner, video, popup
        public string? Position { get; set; }
        public string? ImageUrl { get; set; }
        public string? ClickUrl { get; set; }
        public string? Status { get; set; }          // active, inactive, scheduled, pending

        public int Views { get; set; } = 0;
        public int Clicks { get; set; } = 0;
        public double Ctr { get; set; } = 0;

        // <-- ключевая правка: DateTimeOffset
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public int? Priority { get; set; }
        public string? TargetAudience { get; set; }
    }
}
