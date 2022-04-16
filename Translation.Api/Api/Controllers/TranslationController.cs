using Microsoft.AspNetCore.Mvc;

namespace src.Controllers;

[ApiController]
[Route("[controller]")]
public class TranslationController : ControllerBase
{
    private readonly ILogger<TranslationController> _logger;

    public TranslationController(ILogger<TranslationController> logger)
    {
        _logger = logger;
    }

    [HttpPost("translate")]
    public IActionResult Translate(TranslationRequest request)
    {
        return Ok(request);
    }
}
