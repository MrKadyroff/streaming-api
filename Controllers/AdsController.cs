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

        [HttpPost("with-file")]
        public async Task<IActionResult> CreateWithFile([FromForm] CreateAdWithFileDto dto)
        {
            try
            {
                var ad = await _service.CreateWithFileAsync(dto);
                return Ok(new { success = true, ad, message = "Реклама создана успешно" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ошибка при создании рекламы: " + ex.Message });
            }
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

        [HttpPost("upload-gif")]
        public async Task<IActionResult> UploadGif(IFormFile file)
        {
            try
            {
                var imageUrl = await _service.UploadGifAsync(file);
                return Ok(new { success = true, imageUrl, message = "Изображение загружено успешно" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ошибка при загрузке файла: " + ex.Message });
            }
        }
    }
}
