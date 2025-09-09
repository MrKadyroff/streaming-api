using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/hls")]
    public class HlsController : ControllerBase
    {
        private readonly HlsFileSystemStreamService _hlsService;

        public HlsController(HlsFileSystemStreamService hlsService)
        {
            _hlsService = hlsService;
        }

        [HttpGet("streams")]
        public async Task<IActionResult> GetActiveStreams([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string? status = null)
        {
            var (streams, total) = await _hlsService.GetAllAsync(page, limit, status);
            return Ok(new { streams, total, page, limit });
        }
    }
}
