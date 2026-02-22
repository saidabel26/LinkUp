using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Interfaces.Persistence;

namespace LinkUp.Infrastructure
{
    public class TimedOutGamesBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        // Intervalo en producción
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        private readonly ILogger<TimedOutGamesBackgroundService> _logger;

        public TimedOutGamesBackgroundService(IServiceProvider services, ILogger<TimedOutGamesBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Pequeño retraso inicial
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IBattleshipGameRepository>();
                    var svc = scope.ServiceProvider.GetRequiredService<IBattleshipService>();

                    // Considerar abandono por inactividad mayor a 48 horas
                    var threshold = DateTime.UtcNow.AddHours(-48);
                    var candidates = await repo.GetPotentiallyTimedOutGameIdsAsync(threshold);
                    foreach (var id in candidates)
                    {
                        try
                        {
                            await svc.EvaluateTimeoutAsync(id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error evaluando timeout para la partida {GameId}", id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el ciclo del servicio de timeouts de Battleship");
                }

                try { await Task.Delay(_interval, stoppingToken); } catch { }
            }
        }
    }
}
