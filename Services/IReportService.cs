using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTO;

namespace Services
{
    public interface IReportService
    {
        Task<IEnumerable<ReportDto>> GetAllAsync();
        Task<bool> ResolveAsync(int id, ResolveReportDto dto);
    }
}
