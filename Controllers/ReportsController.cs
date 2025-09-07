using Microsoft.AspNetCore.Mvc;
using Services;
using Models.DTO;

namespace Controllers
{
    [ApiController]
    [Route("api/admin/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;
        public ReportsController(IReportService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _service.GetAllAsync();
            return Ok(new { reports });
        }

        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolveReportDto dto)
        {
            var ok = await _service.ResolveAsync(id, dto);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Жалоба обработана" });
        }
    }
}
