using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Health.Controllers;

[Area("Health")]
[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "LIMS",
            Version = "1.0",
            Timestamp = DateTimeOffset.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Live()
    {
        return Ok(new
        {
            Status = "Alive",
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Ready()
    {
        return Ok(new
        {
            Status = "Ready",
            Timestamp = DateTimeOffset.UtcNow
        });
    }
}
