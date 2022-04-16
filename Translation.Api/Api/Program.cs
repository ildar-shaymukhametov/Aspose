using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<TranslationApiOptions>()
    .Bind(builder.Configuration.GetSection("TranslationApi"))
    .ValidateDataAnnotations();
    
builder.Services.AddHttpClient("TranslationApi", (serviceProvider, httpClient) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TranslationApiOptions>>().Value;
    httpClient.BaseAddress = new Uri(options.Url);
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.Token}");
});

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
