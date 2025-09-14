using Microsoft.AspNetCore.Mvc;
using Services;
using Models.DTO;

namespace Controllers
{
    [ApiController]
    [Route("api/admin/streams")]
    public class StreamsController : ControllerBase
    {
        private readonly IStreamService _service;
        public StreamsController(IStreamService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? status = null)
        {
            var (streams, total) = await _service.GetAllAsync(page, limit, status);
            return Ok(new { streams, total, page, totalPages = (int)Math.Ceiling((double)total / limit) });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFull()
        {
            var items = await _service.GetAllFullAsync();
            return Ok(new { streams = items });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStreamDto dto)
        {
            var stream = await _service.CreateAsync(dto);
            return Ok(new { success = true, stream });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateStreamDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Трансляция обновлена" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Трансляция удалена" });
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var ok = await _service.StartStreamAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Трансляция запущена" });
        }

        [HttpPost("{id}/stop")]
        public async Task<IActionResult> Stop(int id)
        {
            var ok = await _service.StopStreamAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Трансляция остановлена" });
        }
    }
}