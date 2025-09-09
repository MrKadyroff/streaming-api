using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTO;

namespace Services
{
    public interface IAdService
    {
        Task<(IEnumerable<AdDto> Ads, int Total)> GetAllAsync(string? type, string? status);
        Task<AdDto> CreateAsync(CreateAdDto dto);
        Task<bool> UpdateAsync(int id, CreateAdDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<AdDto?> GetByIdAsync(int id);
        Task<object?> GetStatsAsync(int id, string? period);
        Task<bool> IncrementClicksAsync(int id);
    }
}
