using Microsoft.AspNetCore.Mvc;
using Services;
using Models.DTO;

namespace Controllers
{
    [ApiController]
    [Route("api/admin/schedule")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _service;
        public MatchesController(IMatchService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? date = null, [FromQuery] string? sport = null, [FromQuery] string? status = null)
        {
            var (matches, total) = await _service.GetAllAsync(date, sport, status);
            return Ok(new { matches, total });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
        {
            var match = await _service.CreateAsync(dto);
            return Ok(new { success = true, match });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMatchDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Матч обновлен" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Матч удален из расписания" });
        }
    }
}
