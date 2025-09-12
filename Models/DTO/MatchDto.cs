using System;

namespace Models.DTO
{
    public class MatchDto
    {
        public int? Id { get; set; }
        public string? HomeTeam { get; set; }
        public string? AwayTeam { get; set; }
        public DateTime? Date { get; set; }
        public string? Time { get; set; }
        public string? Tournament { get; set; }
        public string? Sport { get; set; }
        public string? Status { get; set; }
        public int? StreamId { get; set; }
        public string? Venue { get; set; }
    }

    public class CreateMatchDto
    {
        public string? HomeTeam { get; set; }
        public string? AwayTeam { get; set; }
        public DateTime? Date { get; set; }
        public string? Time { get; set; }
        public string? Tournament { get; set; }
        public string? Sport { get; set; }
        public string? Venue { get; set; }
    }
}
