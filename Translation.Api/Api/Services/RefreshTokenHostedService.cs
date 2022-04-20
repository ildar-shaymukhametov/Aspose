namespace Translation.Api.Services;

public class RefreshTokenHostedService : IHostedService, IDisposable
{
    private readonly ITokenService _tokenService;
    private Timer _timer = null!;

    public RefreshTokenHostedService(ITokenService tokenService)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        RefreshTokenAccessor.Token = refreshToken;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}