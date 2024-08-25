using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("/")]
public sealed class HelloController : ControllerBase
{
    [HttpGet]
    [Route("hello")]
    public IActionResult ReturnHello()
    {
        Log.Debug("Hello World");
        return Ok();
    }
}