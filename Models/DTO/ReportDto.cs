using System;

namespace Models.DTO
{
    public class ReportDto
    {
        public int? Id { get; set; }
        public string? Type { get; set; }
        public int? TargetId { get; set; }
        public string? TargetType { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime? ReportedAt { get; set; }
    }

    public class ResolveReportDto
    {
        public string? Action { get; set; }
        public string? Comment { get; set; }
    }
}
