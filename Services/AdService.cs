using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

namespace Services
{
    public class AdService : IAdService
    {
        private readonly AppDbContext _db;
        public AdService(AppDbContext db) => _db = db;

        // ---- TZ/UTC helpers ----
        // Пытаемся использовать целевую таймзону для "без оффсета" дат (например, пользователь вводит 01.01.2025 14:30).
        // На Linux id "Asia/Almaty", на Windows он тоже доступен в новых версиях .NET. Если нет — падаем на Local.
        private static readonly TimeZoneInfo? DefaultTz =
            TryGetTz("Asia/Almaty") ?? TimeZoneInfo.Local;

        private static TimeZoneInfo? TryGetTz(string id)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch { return null; }
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                DateTimeKind.Unspecified => // трактуем как время в DefaultTz -> переводим в UTC
                    TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(value, DateTimeKind.Unspecified),
                        DefaultTz ?? TimeZoneInfo.Local),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }

        private static DateTime EnsureUtcOr(DateTime? value, DateTime utcDefault)
            => value.HasValue ? EnsureUtc(value.Value) : utcDefault;

        public async Task<(IEnumerable<AdDto> Ads, int Total)> GetAllAsync(string? type, string? status)
        {
            var query = _db.Ads.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(a => a.Type == type);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            var total = await query.CountAsync();

            var ads = await query
                .Select(a => new AdDto
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
                    // Npgsql вернёт UTC DateTime для timestamptz
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Priority = a.Priority,
                    TargetAudience = a.TargetAudience
                })
                .ToListAsync();

            return (ads, total);
        }

        public async Task<AdDto> CreateAsync(CreateAdDto dto)
        {
            var nowUtc = DateTime.UtcNow;
            var defaultEndUtc = nowUtc.AddDays(30);

            var ad = new Models.Entities.Ad
            {
                Title = dto.Title ?? "",
                Type = dto.Type ?? "",
                Position = dto.Position ?? "",
                ImageUrl = dto.ImageUrl ?? "",
                ClickUrl = dto.ClickUrl ?? "",
                Status = "active",

                // КРИТИЧНО: нормализуем к UTC перед сохранением
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,

                Priority = dto.Priority ?? 0,
                TargetAudience = dto.TargetAudience ?? "",
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
                Status = ad.Status,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate
            };
        }

        public async Task<AdDto> CreateWithFileAsync(CreateAdWithFileDto dto)
        {
            var nowUtc = DateTime.UtcNow;
            string? imageUrl = null;

            // Если есть файл, загружаем его
            if (dto.ImageFile != null)
            {
                imageUrl = await UploadGifAsync(dto.ImageFile);
            }

            var ad = new Models.Entities.Ad
            {
                Title = dto.Title ?? "",
                Type = dto.Type ?? "",
                Position = dto.Position ?? "",
                ImageUrl = imageUrl ?? dto.ImageUrl ?? "",
                ClickUrl = dto.ClickUrl ?? "",
                Status = "active",

                StartDate = dto.StartDate,
                EndDate = dto.EndDate,

                Priority = dto.Priority ?? 0,
                TargetAudience = dto.TargetAudience ?? "",
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

        public async Task<bool> UpdateAsync(int id, CreateAdDto dto)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return false;

            ad.Title = dto.Title ?? ad.Title;
            ad.Type = dto.Type ?? ad.Type;
            ad.Position = dto.Position ?? ad.Position;
            ad.ImageUrl = dto.ImageUrl ?? ad.ImageUrl;
            ad.ClickUrl = dto.ClickUrl ?? ad.ClickUrl;

            if (dto.StartDate.HasValue)
                ad.StartDate = dto.StartDate.Value;

            if (dto.EndDate.HasValue)
                ad.EndDate = dto.EndDate.Value;

            ad.Priority = dto.Priority ?? ad.Priority;
            ad.TargetAudience = dto.TargetAudience ?? ad.TargetAudience;

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
            var ad = await _db.Ads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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
                StartDate = ad.StartDate, // уже UTC из БД
                EndDate = ad.EndDate,
                Priority = ad.Priority,
                TargetAudience = ad.TargetAudience
            };
        }

        public async Task<object?> GetStatsAsync(int id, string? period)
        {
            var ad = await _db.Ads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (ad == null) return null;

            return new
            {
                stats = new
                {
                    views = ad.Views,
                    clicks = ad.Clicks,
                    ctr = ad.Ctr,
                    revenue = 0
                }
            };
        }

        public async Task<bool> IncrementClicksAsync(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return false;

            ad.Clicks++;
            if (ad.Views > 0)
                ad.Ctr = (double)ad.Clicks / ad.Views;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string> UploadGifAsync(IFormFile gifFile)
        {
            if (gifFile == null || gifFile.Length == 0)
                throw new ArgumentException("Файл не может быть пустым");

            // Проверяем тип файла
            var allowedTypes = new[] { "image/gif", "image/png", "image/jpeg", "image/jpg" };
            if (!allowedTypes.Contains(gifFile.ContentType.ToLower()))
                throw new ArgumentException("Поддерживаются только GIF, PNG, JPG файлы");

            // Проверяем размер файла (максимум 10MB)
            if (gifFile.Length > 10 * 1024 * 1024)
                throw new ArgumentException("Размер файла не должен превышать 10MB");

            // Создаем папку если её нет
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "ads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            // Генерируем уникальное имя файла
            var fileExtension = Path.GetExtension(gifFile.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            // Сохраняем файл
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await gifFile.CopyToAsync(fileStream);
            }

            // Возвращаем URL для фронтенда
            return $"/images/ads/{fileName}";
        }
    }
}
