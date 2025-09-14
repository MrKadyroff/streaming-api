using System;

namespace Models.Entities
{
    public class User
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; } // admin, moderator, user
        public string? Status { get; set; } // active, banned, pending
        public DateTime? RegisteredAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
