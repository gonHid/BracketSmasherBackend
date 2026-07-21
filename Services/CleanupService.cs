using BracketSmasherBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace BracketSmasherBackend.Services;

public class CleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CleanupService> _logger;

    public CleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<CleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CleanupService iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredMatches();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando limpieza");
            }

            // esperar 24 horas
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task CleanupExpiredMatches()
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        var expired = await db.MatchSessions
            .Where(x => x.ExpiresAt < now)
            .ToListAsync();

        if (expired.Count == 0)
        {
            _logger.LogInformation("No hay salas expiradas");
            return;
        }

        db.MatchSessions.RemoveRange(expired);

        await db.SaveChangesAsync();

        _logger.LogInformation(
            "Se eliminaron {Count} salas expiradas",
            expired.Count
        );
    }
}