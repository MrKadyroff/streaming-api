using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;

namespace Services
{
    public class MatchService : IMatchService
    {
        private readonly AppDbContext _db;
        public MatchService(AppDbContext db) => _db = db;

        public async Task<(IEnumerable<MatchDto> Matches, int Total)> GetAllAsync(string? date, string? sport, string? status)
        {
            var query = _db.Matches.AsQueryable();
            if (!string.IsNullOrEmpty(date))
                query = query.Where(m => m.Date.ToString("yyyy-MM-dd") == date);
            if (!string.IsNullOrEmpty(sport))
                query = query.Where(m => m.Sport == sport);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(m => m.Status == status);
            var total = await query.CountAsync();
            var matches = await query.Select(m => new MatchDto
            {
                Id = m.Id,
                HomeTeam = m.HomeTeam,
                AwayTeam = m.AwayTeam,
                Date = m.Date,
                Time = m.Time,
                Tournament = m.Tournament,
                Sport = m.Sport,
                Status = m.Status,
                StreamId = m.StreamId,
                Venue = m.Venue
            }).ToListAsync();
            return (matches, total);
        }

        public async Task<MatchDto> CreateAsync(CreateMatchDto dto)
        {
            var match = new Models.Entities.Match
            {
                HomeTeam = dto.HomeTeam,
                AwayTeam = dto.AwayTeam,
                Date = dto.Date,
                Time = dto.Time,
                Tournament = dto.Tournament,
                Sport = dto.Sport,
                Status = "upcoming",
                Venue = dto.Venue
            };
            _db.Matches.Add(match);
            await _db.SaveChangesAsync();
            return new MatchDto
            {
                Id = match.Id,
                HomeTeam = match.HomeTeam,
                AwayTeam = match.AwayTeam,
                Status = match.Status
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateMatchDto dto)
        {
            var match = await _db.Matches.FindAsync(id);
            if (match == null) return false;
            match.HomeTeam = dto.HomeTeam;
            match.AwayTeam = dto.AwayTeam;
            match.Date = dto.Date;
            match.Time = dto.Time;
            match.Tournament = dto.Tournament;
            match.Sport = dto.Sport;
            match.Venue = dto.Venue;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var match = await _db.Matches.FindAsync(id);
            if (match == null) return false;
            _db.Matches.Remove(match);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
