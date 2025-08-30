using Microsoft.AspNetCore.Mvc;
using StreamApi.Filters;
using StreamApi.Services;

namespace StreamApi.Controllers;

[ApiController]
[Route("streams")]
public class StreamsController : ControllerBase
{
    private readonly IStreamService _svc;
    public StreamsController(IStreamService svc) => _svc = svc;

    // требует заголовок Authorization: Bearer <AdminToken>
    [HttpGet]
    [AdminAuthorize]
    public IActionResult List() => Ok(_svc.ListStreams());
}
