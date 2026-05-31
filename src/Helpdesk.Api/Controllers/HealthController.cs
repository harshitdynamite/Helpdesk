using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>Liveness probe. Returns 200 with basic status info.</summary>
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "healthy",
        service = "Helpdesk.Api",
        timeUtc = DateTime.UtcNow
    });
}
