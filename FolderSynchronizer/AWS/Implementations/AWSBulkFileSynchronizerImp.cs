using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Enums;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSBulkFileSynchronizerImp : IAWSBulkFileSynchronizer
    {
        private IAWSFileSyncChecker FileSyncChecker { get; }
        private IAWSFileUploader Uploader { get; }
        private IAWSFileDeleter Deleter { get; }

        public AWSBulkFileSynchronizerImp(IAWSFileSyncChecker awsFileSyncChecker, IAWSFileUploader uploader, IAWSFileDeleter deleter)
        {
            FileSyncChecker = awsFileSyncChecker ?? throw new ArgumentNullException(nameof(awsFileSyncChecker));
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
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
            var action = GetNeededActionForFile(file);
            switch (action)
            {
                case AWSSynchronizationAction.None:
                    return;

                case AWSSynchronizationAction.Upload:
                    if (file.SourceData == null)
                    {
                        throw new ArgumentNullException(nameof(file.SourceData), "To upload a file, we need a local path");
                    }

                    await Uploader.UploadFileAsync(file.SourceData.Path);
                    break;

                case AWSSynchronizationAction.Delete:
                    if (file.DestinationData == null)
                    {
                        throw new ArgumentNullException(nameof(file.DestinationData), "To delete a file, we need a remote path");
                    }

                    await Deleter.DeleteRemoteFileAsync(file.DestinationData.Path);
                    break;
            }
        }

        private AWSSynchronizationAction GetNeededActionForFile(FileSynchronizationStatusData file)
        {
            if (file.DestinationData == null)
            {
                return AWSSynchronizationAction.Upload;
            }

            if (file.SourceData == null)
            {
                return AWSSynchronizationAction.Delete;
            }

            if (file.SourceData.LastModifiedDate > file.DestinationData.LastModifiedDate)
            {
                return AWSSynchronizationAction.Upload;
            }

            return AWSSynchronizationAction.None;
        }
    }
}