using System;

namespace Models.Entities
{
    public class Report
    {
        public int? Id { get; set; }
        public string? Type { get; set; } // inappropriate_content, etc.
        public int? TargetId { get; set; }
        public string? TargetType { get; set; } // stream, user, etc.
        public string? Reason { get; set; }
        public string? Status { get; set; } // pending, resolved
        public DateTime ReportedAt { get; set; }
    }
}
