using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTO;

namespace Services
{
    public interface IUserService
    {
        Task<(IEnumerable<UserDto> Users, int Total)> GetAllAsync(string? role, string? status);
        Task<bool> BanAsync(int id, UpdateUserStatusDto dto);
        Task<bool> UnbanAsync(int id);
    }
}
