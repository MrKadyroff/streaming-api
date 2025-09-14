using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.DTO;
using Models;

namespace Services
{
    public class StreamService : Services.IStreamService
    {
        private readonly AppDbContext _db;
        public StreamService(AppDbContext db) => _db = db;

        public async Task<(IEnumerable<StreamInfoDto> Streams, int Total)> GetAllAsync(int page, int limit, string? status)
        {
            var query = _db.Streams.AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(s => s.Status == status);
            var total = await query.CountAsync();
            var streams = await query.Skip((page - 1) * limit).Take(limit)
                .Select(s => new Models.DTO.StreamInfoDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Status = s.Status,
                    Viewers = s.Viewers,
                    StreamUrl = s.StreamUrl,
                    FallbackUrl = s.FallbackUrl,
                    StartTime = s.StartTime,
                    Quality = s.Quality
                }).ToListAsync();
            return (streams, total);
        }

        public async Task<Models.DTO.StreamInfoDto?> GetByIdAsync(int id)
        {
            var s = await _db.Streams.FindAsync(id);
            if (s == null) return null;
            return new Models.DTO.StreamInfoDto
            {
                Id = s.Id,
                Title = s.Title,
                Status = s.Status,
                Viewers = s.Viewers,
                StreamUrl = s.StreamUrl,
                FallbackUrl = s.FallbackUrl,
                StartTime = s.StartTime,
                Quality = s.Quality
            };
        }

        public async Task<Models.DTO.StreamInfoDto> CreateAsync(Models.DTO.CreateStreamDto dto)
        {
            var stream = new Models.Entities.Stream
            {
                Title = dto.Title ?? "",
                Description = dto.Description ?? "",
                StreamUrl = dto.StreamUrl ?? "",
                FallbackUrl = dto.FallbackUrl ?? "",
                ScheduledTime = dto.ScheduledTime,
                Status = "upcoming",
                Sport = dto.Sport ?? "",
                Tournament = dto.Tournament ?? "",
                HomeTeam = dto.HomeTeam ?? "",
                AwayTeam = dto.AwayTeam ?? "",
                CreatedAt = DateTime.UtcNow,
                Quality = new List<string> { "1080p", "720p", "480p" }
            };
            _db.Streams.Add(stream);
            await _db.SaveChangesAsync();
            return new Models.DTO.StreamInfoDto
            {
                Id = stream.Id,
                Title = stream.Title,
                Status = stream.Status,
                Viewers = stream.Viewers,
                StreamUrl = stream.StreamUrl,
                FallbackUrl = stream.FallbackUrl,
                StartTime = stream.StartTime,
                Quality = stream.Quality
            };
        }

        public async Task<bool> UpdateAsync(int id, Models.DTO.CreateStreamDto dto)
        {
            var s = await _db.Streams.FindAsync(id);
            if (s == null) return false;
            s.Title = dto.Title ?? s.Title;
            s.Description = dto.Description ?? s.Description;
            s.StreamUrl = dto.StreamUrl ?? s.StreamUrl;
            s.FallbackUrl = dto.FallbackUrl ?? s.FallbackUrl;
            s.ScheduledTime = dto.ScheduledTime ?? s.ScheduledTime;
            s.Sport = dto.Sport ?? s.Sport;
            s.Tournament = dto.Tournament ?? s.Tournament;
            s.HomeTeam = dto.HomeTeam ?? s.HomeTeam;
            s.AwayTeam = dto.AwayTeam ?? s.AwayTeam;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var s = await _db.Streams.FindAsync(id);
            if (s == null) return false;
            _db.Streams.Remove(s);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StartStreamAsync(int id)
        {
            var s = await _db.Streams.FindAsync(id);
            if (s == null) return false;
            s.Status = "live";
            s.StartTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StopStreamAsync(int id)
        {
            var s = await _db.Streams.FindAsync(id);
            if (s == null) return false;
            s.Status = "finished";
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
