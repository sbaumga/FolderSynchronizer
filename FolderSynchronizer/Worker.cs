using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer
{
    public class Worker : BackgroundService
    {
        private IAWSBulkFileSynchronizer AWSBulkFileSynchronizer { get; }
        private ISavedFileListBulkSynchronizer SavedFileListBulkSynchronizer { get; }

        public Worker(IAWSBulkFileSynchronizer awsBulkFileSynchronizer, ISavedFileListBulkSynchronizer savedFileListBulkSynchronizer)
        {
            AWSBulkFileSynchronizer = awsBulkFileSynchronizer ?? throw new ArgumentNullException(nameof(awsBulkFileSynchronizer));
            SavedFileListBulkSynchronizer = savedFileListBulkSynchronizer ?? throw new ArgumentException(nameof(savedFileListBulkSynchronizer));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoInitialSync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);

                await SavedFileListBulkSynchronizer.SynchronizeFilesAsync();
            }
        }

        private async Task DoInitialSync()
        {
            await AWSBulkFileSynchronizer.SynchronizeFilesAsync();
            await SavedFileListBulkSynchronizer.SynchronizeFilesAsync();
        }
    }
}