using System.Diagnostics;
using Client;
using Microsoft.AspNetCore.Mvc;
using Parser;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly IParser _parser;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITranslationApiClient _translationApiClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IParser parser, IHttpClientFactory httpClientFactory, ITranslationApiClient translationApiClient, ILogger<HomeController> logger)
    {
        _parser = parser;
        _httpClientFactory = httpClientFactory;
        _translationApiClient = translationApiClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        await SetViewDataAsync();
        return View();
    }

    private async Task SetViewDataAsync()
    {
        var languageViewModels = await GetLanguagesAsync();
        if (languageViewModels != null)
        {
            ViewData["Languages"] = languageViewModels;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Translate(TransaleFileViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await SetViewDataAsync();
            return View(nameof(Index));
        }

        try
        {
            await using var stream = await GetStreamAsync(viewModel);
            var texts = _parser.Parse(stream);

            var translations = await _translationApiClient.TranslateAsync(texts, viewModel.SourceLanguage, viewModel.TargetLanguage);

            return View("Result", new TranlsationResultViewModel
            {
                Translations = translations,
                OriginalTexts = texts
            });
        }
        catch (Exception ex)
        {
            var fileName = viewModel.File?.FileName ?? viewModel.Url;
            _logger.LogError(ex, "Failed to translate file {file} from {sourceLanguage} to {targetLanguage}", fileName, viewModel.SourceLanguage, viewModel.TargetLanguage);
            return Content("Sorry, we were unable to translate your file. Please try again later");
        }
    }

    private async Task<Stream> GetStreamAsync(TransaleFileViewModel viewModel)
    {
        if (viewModel.File != null)
        {
            return viewModel.File.OpenReadStream();
        }

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(viewModel.Url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<LanguageViewModel[]?> GetLanguagesAsync()
    {
        try
        {
            var supportedLanguages = await _translationApiClient.LanguagesAsync();
            var viewModels = supportedLanguages?.Select(x => new LanguageViewModel
            {
                Code = x.Code,
                Name = x.Name
            })?.ToArray();

            return viewModels;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch languages");
            return null;
        }
    }
}
