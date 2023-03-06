using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer
{
    public class Worker : BackgroundService
    {
        private bool PeriodicQueuePollingOn { get; }
        private int ExecutionIntervalSeconds { get; }

        private IAWSBulkFileSynchronizer AWSBulkFileSynchronizer { get; }
        private ISavedFileListBulkSynchronizer SavedFileListBulkSynchronizer { get; }
        private IAWSSQSPoller Poller { get; }

        public Worker(LocalConfigData configData, IAWSBulkFileSynchronizer awsBulkFileSynchronizer, ISavedFileListBulkSynchronizer savedFileListBulkSynchronizer, IAWSSQSPoller poller)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            PeriodicQueuePollingOn = configData.PeriodicQueuePollingOn;
            ExecutionIntervalSeconds = configData.ExecutionIntervalSeconds;

            AWSBulkFileSynchronizer = awsBulkFileSynchronizer ?? throw new ArgumentNullException(nameof(awsBulkFileSynchronizer));
            SavedFileListBulkSynchronizer = savedFileListBulkSynchronizer ?? throw new ArgumentNullException(nameof(savedFileListBulkSynchronizer));
            Poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await DoInitialSync();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(ExecutionIntervalSeconds * 1000, stoppingToken);

                    if (PeriodicQueuePollingOn)
                    {
                        await Poller.GetMessagesAsync();
                    }

                    await SavedFileListBulkSynchronizer.SynchronizeFilesAsync();
                }
            } catch(Exception ex)
            {
                throw;
            }
        }

        private async Task DoInitialSync()
        {
            await Poller.GetMessagesAsync();

            await AWSBulkFileSynchronizer.SynchronizeFilesAsync();
            await SavedFileListBulkSynchronizer.SynchronizeFilesAsync();
        }
    }
}