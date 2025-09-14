using System;

namespace Models.Entities
{
    public class Match
    {
        public int? Id { get; set; }
        public string? HomeTeam { get; set; }
        public string? AwayTeam { get; set; }
        public DateTime? Date { get; set; }
        public string? Time { get; set; }
        public string? Tournament { get; set; }
        public string? Sport { get; set; }
        public string? Status { get; set; } // upcoming, live, finished
        public int? StreamId { get; set; }
        public string? Venue { get; set; }
    }
}
