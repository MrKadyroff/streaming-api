using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

namespace Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        public ReportService(AppDbContext db) => _db = db;

        public async Task<IEnumerable<ReportDto>> GetAllAsync()
        {
            return await _db.Reports.Select(r => new ReportDto
            {
                Id = r.Id,
                Type = r.Type,
                TargetId = r.TargetId,
                TargetType = r.TargetType,
                Reason = r.Reason,
                Status = r.Status,
                ReportedAt = r.ReportedAt
            }).ToListAsync();
        }

        public async Task<bool> ResolveAsync(int id, ResolveReportDto dto)
        {
            var report = await _db.Reports.FindAsync(id);
            if (report == null) return false;
            report.Status = "resolved";
            // Можно добавить логику для action/comment
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
