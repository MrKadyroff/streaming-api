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
        public UsersController(IUserService service) => _service = service;

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
    }
}
