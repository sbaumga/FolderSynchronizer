using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Enums;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSBulkFileSynchronizerImp : IAWSBulkFileSynchronizer
    {
        private IAWSFileSyncChecker FileSyncChecker { get; }
        private IAWSFileUploader Uploader { get; }
        private IAWSFileDeleter Deleter { get; }
        private ISynchronizationActionDecider SyncActionDecider { get; }

        public AWSBulkFileSynchronizerImp(
            IAWSFileSyncChecker awsFileSyncChecker,
            IAWSFileUploader uploader,
            IAWSFileDeleter deleter,
            ISynchronizationActionDecider synchronizationActionDecider)
        {
            FileSyncChecker = awsFileSyncChecker ?? throw new ArgumentNullException(nameof(awsFileSyncChecker));
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
            SyncActionDecider = synchronizationActionDecider ?? throw new ArgumentNullException(nameof(synchronizationActionDecider));
        }

        public async Task SynchronizeFilesAsync()
        {
            var syncStatus = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            var syncTasks = new List<Task>();
            foreach (var file in syncStatus)
            {
                syncTasks.Add(TakeActionOnFileIfNeeded(file));
            }

            Task.WaitAll(syncTasks.ToArray());
        }

        private async Task TakeActionOnFileIfNeeded(FileSynchronizationStatusData file)
        {
            var action = SyncActionDecider.GetNeededActionForFile(file);
            switch (action)
            {
                case FileSynchronizationAction.None:
                    return;

                case FileSynchronizationAction.Upload:
                    if (file.SourceData == null)
                    {
                        throw new AWSFileSynchronizationException($"An {FileSynchronizationAction.Upload} action was returned with no source file to upload");
                    }

                    await Uploader.UploadFileAsync(file.SourceData.Path);
                    break;

                case FileSynchronizationAction.Delete:
                    if (file.DestinationData == null)
                    {
                        throw new AWSFileSynchronizationException($"An {FileSynchronizationAction.Delete} action was returned with no destination file to delete");
                    }

                    await Deleter.DeleteRemoteFileAsync(file.DestinationData.Path);
                    break;
            }
        }
    }
}