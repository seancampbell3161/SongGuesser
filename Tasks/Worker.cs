using API.Interfaces;

namespace Tasks;

public class Worker(IServiceProvider services, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = services.CreateScope();
            
            var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();

            try
            {
                await repo.AddNewSongOfTheDayAsync();
            }
            catch (Exception)
            {
                logger.LogError("Error occurred adding new song of the day");
            }
        }
    }
}