using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("API OK");
}
