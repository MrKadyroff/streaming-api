using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

namespace Services
{
    public class AdService : IAdService
    {
        private readonly AppDbContext _db;
        public AdService(AppDbContext db) => _db = db;

        public async Task<(IEnumerable<AdDto> Ads, int Total)> GetAllAsync(string? type, string? status)
        {
            var query = _db.Ads.AsQueryable();
            if (!string.IsNullOrEmpty(type))
                query = query.Where(a => a.Type == type);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);
            var total = await query.CountAsync();
            var ads = await query.Select(a => new AdDto
            {
                Id = a.Id,
                Title = a.Title,
                Type = a.Type,
                Position = a.Position,
                ImageUrl = a.ImageUrl,
                ClickUrl = a.ClickUrl,
                Status = a.Status,
                Views = a.Views,
                Clicks = a.Clicks,
                Ctr = a.Ctr,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Priority = a.Priority,
                TargetAudience = a.TargetAudience
            }).ToListAsync();
            return (ads, total);
        }

        public async Task<AdDto> CreateAsync(CreateAdDto dto)
        {
            var ad = new Models.Entities.Ad
            {
                Title = dto.Title,
                Type = dto.Type,
                Position = dto.Position,
                ImageUrl = dto.ImageUrl,
                ClickUrl = dto.ClickUrl,
                Status = "pending",
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Priority = dto.Priority,
                TargetAudience = dto.TargetAudience,
                Views = 0,
                Clicks = 0,
                Ctr = 0
            };
            _db.Ads.Add(ad);
            await _db.SaveChangesAsync();
            return new AdDto
            {
                Id = ad.Id,
                Title = ad.Title,
                Status = ad.Status
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateAdDto dto)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return false;
            ad.Title = dto.Title;
            ad.Type = dto.Type;
            ad.Position = dto.Position;
            ad.ImageUrl = dto.ImageUrl;
            ad.ClickUrl = dto.ClickUrl;
            ad.StartDate = dto.StartDate;
            ad.EndDate = dto.EndDate;
            ad.Priority = dto.Priority;
            ad.TargetAudience = dto.TargetAudience;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return false;
            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return false;
            ad.Status = "active";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return false;
            ad.Status = "inactive";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<AdDto?> GetByIdAsync(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return null;
            return new AdDto
            {
                Id = ad.Id,
                Title = ad.Title,
                Type = ad.Type,
                Position = ad.Position,
                ImageUrl = ad.ImageUrl,
                ClickUrl = ad.ClickUrl,
                Status = ad.Status,
                Views = ad.Views,
                Clicks = ad.Clicks,
                Ctr = ad.Ctr,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                Priority = ad.Priority,
                TargetAudience = ad.TargetAudience
            };
        }

        public async Task<object?> GetStatsAsync(int id, string? period)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return null;
            // Пример: просто возвращаем текущие значения, можно расширить под реальные отчёты
            return new
            {
                stats = new
                {
                    views = ad.Views,
                    clicks = ad.Clicks,
                    ctr = ad.Ctr,
                    revenue = 0,
                    dailyStats = new[] { new { date = ad.StartDate.ToString("yyyy-MM-dd"), views = ad.Views, clicks = ad.Clicks } }
                }
            };
        }

        public async Task<bool> IncrementClicksAsync(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return false;

            ad.Clicks++;
            // Обновляем CTR (Click Through Rate) = Clicks / Views
            if (ad.Views > 0)
                ad.Ctr = (double)ad.Clicks / ad.Views;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
