using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db) => _db = db;

        public async Task<(IEnumerable<UserDto> Users, int Total)> GetAllAsync(string? role, string? status)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);
            var total = await query.CountAsync();
            var users = await query.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                Status = u.Status,
                RegisteredAt = u.RegisteredAt,
                LastLogin = u.LastLogin
            }).ToListAsync();
            return (users, total);
        }

        public async Task<bool> BanAsync(int id, UpdateUserStatusDto dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return false;
            user.Status = "banned";
            // Можно добавить логику для duration/reason
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnbanAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return false;
            user.Status = "active";
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
