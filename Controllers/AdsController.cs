using Microsoft.AspNetCore.Mvc;
using Services;
using Models.DTO;

namespace Controllers
{
    [ApiController]
    [Route("api/admin/ads")]
    public class AdsController : ControllerBase
    {
        private readonly IAdService _service;
        public AdsController(IAdService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? type = null, [FromQuery] string? status = null)
        {
            var (ads, total) = await _service.GetAllAsync(type, status);
            return Ok(new { ads, total });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdDto dto)
        {
            var ad = await _service.CreateAsync(dto);
            return Ok(new { success = true, ad });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateAdDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Реклама обновлена" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Реклама удалена" });
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var ok = await _service.ActivateAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Реклама активирована" });
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var ok = await _service.DeactivateAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Реклама деактивирована" });
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> Stats(int id, [FromQuery] string? period = null)
        {
            var stats = await _service.GetStatsAsync(id, period);
            if (stats == null) return NotFound();
            return Ok(stats);
        }
    }
}
