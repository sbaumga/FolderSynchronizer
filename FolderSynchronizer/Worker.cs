namespace FolderSynchronizer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        private FolderWatcher Watcher { get; }

        public Worker(ILogger<Worker> logger, FolderWatcher watcher)
        {
            _logger = logger;

            Watcher = watcher;

            // TODO: add initial startup sync
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}