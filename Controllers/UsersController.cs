using Microsoft.AspNetCore.Mvc;
using Services;
using Models.DTO;

namespace Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IOnlineTracker _tracker;

        public UsersController(IUserService service, IOnlineTracker tracker)
        {
            _service = service;
            _tracker = tracker;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? role = null, [FromQuery] string? status = null)
        {
            var (users, total) = await _service.GetAllAsync(role, status);
            return Ok(new { users, total });
        }

        [HttpPut("{id}/ban")]
        public async Task<IActionResult> Ban(int id, [FromBody] UpdateUserStatusDto dto)
        {
            var ok = await _service.BanAsync(id, dto);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Пользователь заблокирован" });
        }

        [HttpPut("{id}/unban")]
        public async Task<IActionResult> Unban(int id)
        {
            var ok = await _service.UnbanAsync(id);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Пользователь разблокирован" });
        }

        [HttpGet("online-count")]
        public IActionResult GetOnlineCount()
        {
            var count = _tracker.Count;
            return Ok(new { onlineCount = count });
        }
    }
}
