using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Translation.Api.Controllers;

[ApiController]
public class TranslationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TranslationApiOptions _options;
    private readonly ILogger<TranslationController> _logger;

    public TranslationController(IHttpClientFactory httpClientFactory, IOptions<TranslationApiOptions> options, ILogger<TranslationController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    [HttpPost("translate")]
    public async Task<IActionResult> Translate(TranslationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Translate");
            var message = CreateTranslationRequestMessage(request);
            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request: {request}", request);
            return Problem(title: ex.Message, detail: ex.StackTrace);
        }
    }

    [HttpGet("languages")]
    public async Task<IActionResult> Languages()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("Languages");
            var message = CreateLanguagesRequestMessage();
            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while requesting the list of supported languages");
            return Problem(title: ex.Message, detail: ex.StackTrace);
        }
    }

    private HttpRequestMessage CreateTranslationRequestMessage(TranslationRequest request)
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = JsonContent.Create(new
            {
                folderId = _options.FolderId,
                texts = request.Texts,
                targetLanguageCode = request.TargetLanguageCode,
                sourceLanguageCode = request.SourceLanguageCode
            })
        };
    }

    private HttpRequestMessage CreateLanguagesRequestMessage()
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = JsonContent.Create(new
            {
                folderId = _options.FolderId
            })
        };
    }
}