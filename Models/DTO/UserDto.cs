using System;

namespace Models.DTO
{
    public class UserDto
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class UpdateUserStatusDto
    {
        public string? Reason { get; set; }
        public string? Duration { get; set; }
    }
}
