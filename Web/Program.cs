using Client;
using Microsoft.Extensions.Options;
using Parser;
using Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IParser, Parser.Parser>();

builder.Services.AddOptions<TranslationApiOptions>()
    .Bind(builder.Configuration.GetSection("TranslationApi"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<ITranslationApiClient>(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TranslationApiOptions>>().Value;
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    httpClient.BaseAddress = new Uri(options.Url!);
    httpClient.DefaultRequestHeaders.Add("X-Translation-Api-Key", options.ApiKey);

    return new TranslationApiClient(httpClient);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
