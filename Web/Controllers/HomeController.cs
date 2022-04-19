using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly IParser _parser;
    private readonly ITranslationApiClient _translationApiClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IParser parser, ITranslationApiClient translationApiClient, ILogger<HomeController> logger)
    {
        _parser = parser;
        _translationApiClient = translationApiClient;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Translate(TransaleFileViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Index));
        }

        try
        {
            using var stream = viewModel.File.OpenReadStream();
            var texts = _parser.Parse(stream);

            var translations = await _translationApiClient.TranslateAsync(texts, viewModel.SourceLanguage, viewModel.TargetLanguage);

            return View("Result", new TranlsationResultViewModel
            {
                FileName = viewModel.File.FileName,
                Translations = translations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to translate file {file} from {sourceLanguage} to {targetLanguage}", viewModel.File.FileName, viewModel.SourceLanguage, viewModel.TargetLanguage);
            return Content("Sorry, we were unable to translate your file. Please try again later");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
