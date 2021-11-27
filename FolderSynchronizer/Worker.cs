using FolderSynchronizer.AWS;

namespace FolderSynchronizer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        private AWSBulkFileSynchronizer BulkFileSynchronizer { get; }
        private FolderWatcher Watcher { get; }

        public Worker(ILogger<Worker> logger, AWSBulkFileSynchronizer bulkFileSynchronizer, FolderWatcher watcher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            BulkFileSynchronizer = bulkFileSynchronizer ?? throw new ArgumentNullException(nameof(bulkFileSynchronizer));
            Watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
        }

        private async Task DoInitialSync()
        {
            await BulkFileSynchronizer.SynchronizeFiles();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = DoInitialSync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}