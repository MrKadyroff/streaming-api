using System;
using System.Collections.Generic;

namespace Models.Entities
{
    public class Stream
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StreamUrl { get; set; }
        public string FallbackUrl { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public DateTime? StartTime { get; set; }
        public string Status { get; set; } // live, upcoming, finished
        public string Sport { get; set; }
        public string Tournament { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int Viewers { get; set; }
        public List<string> Quality { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
