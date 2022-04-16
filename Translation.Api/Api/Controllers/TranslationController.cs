using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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

        var message = new HttpRequestMessage
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

        try
        {
            var httpClient = _httpClientFactory.CreateClient("TranslationApi");
            var response = await httpClient.SendAsync(message);
            var result = await response.Content.ReadFromJsonAsync<TranslationResult>();
            var translations = result.Translations.Select(x => x.Text).ToArray();

            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request: {request}", request);
            return Problem(title: ex.Message, detail: ex.StackTrace);
        }
    }
}
