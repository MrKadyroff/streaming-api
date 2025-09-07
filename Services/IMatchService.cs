using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTO;

namespace Services
{
    public interface IMatchService
    {
        Task<(IEnumerable<MatchDto> Matches, int Total)> GetAllAsync(string? date, string? sport, string? status);
        Task<MatchDto> CreateAsync(CreateMatchDto dto);
        Task<bool> UpdateAsync(int id, CreateMatchDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
