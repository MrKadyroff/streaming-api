using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;
using Models.DTO;

namespace Services
{
    public interface IStreamService
    {
        Task<(IEnumerable<StreamInfoDto> Streams, int Total)> GetAllAsync(int page, int limit, string? status);
        Task<StreamInfoDto?> GetByIdAsync(int id);
        Task<StreamInfoDto> CreateAsync(CreateStreamDto dto);
        Task<bool> UpdateAsync(int id, CreateStreamDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> StartStreamAsync(int id);
        Task<bool> StopStreamAsync(int id);
        Task<IEnumerable<Models.Entities.Stream>> GetAllFullAsync();
    }
}
