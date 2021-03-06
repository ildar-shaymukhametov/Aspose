using Microsoft.Extensions.Options;
using Translation.Api;
using Translation.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<TranslationApiOptions>()
    .Bind(builder.Configuration.GetSection("TranslationApi"))
    .ValidateDataAnnotations();

builder.Services.AddOptions<RefreshTokenApiOptions>()
    .Bind(builder.Configuration.GetSection("RefreshTokenApi"))
    .ValidateDataAnnotations();

builder.Services.AddHttpClient("Translate", (serviceProvider, httpClient) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TranslationApiOptions>>().Value;
    httpClient.BaseAddress = new Uri(options.TranslateUrl);
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RefreshTokenAccessor.Token}");
});

builder.Services.AddHttpClient("Languages", (serviceProvider, httpClient) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TranslationApiOptions>>().Value;
    httpClient.BaseAddress = new Uri(options.LanguagesUrl);
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {RefreshTokenAccessor.Token}");
});

builder.Services.AddHttpClient("RefreshTokenApi", (serviceProvider, httpClient) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<RefreshTokenApiOptions>>().Value;
    httpClient.BaseAddress = new Uri(options.Url);
});

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddHostedService<RefreshTokenHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
