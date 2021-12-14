using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS
{
    public class AWSBulkFileSynchronizer
    {
        private IAWSFileSyncChecker FileSyncChecker { get; }
        private IAWSFileUploader Uploader { get; }
        private IAWSFileDeleter Deleter { get; }

        public AWSBulkFileSynchronizer(IAWSFileSyncChecker awsFileSyncChecker, IAWSFileUploader uploader, IAWSFileDeleter deleter)
        {
            FileSyncChecker = awsFileSyncChecker ?? throw new ArgumentNullException(nameof(awsFileSyncChecker));
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
        }

        public async Task SynchronizeFiles()
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
                    if (file.LocalData == null)
                    {
                        throw new ArgumentNullException(nameof(file.LocalData), "To upload a file, we need a local path");
                    }

                    await Uploader.UploadFileAsync(file.LocalData.Path);
                    break;

                case AWSSynchronizationAction.Delete:
                    if (file.RemoteData == null)
                    {
                        throw new ArgumentNullException(nameof(file.RemoteData), "To delete a file, we need a remote path");
                    }

                    await Deleter.DeleteRemoteFileAsync(file.RemoteData.Path);
                    break;
            }
        }

        private AWSSynchronizationAction GetNeededActionForFile(FileSynchronizationStatusData file)
        {
            if (file.RemoteData == null)
            {
                return AWSSynchronizationAction.Upload;
            }

            if (file.LocalData == null)
            {
                return AWSSynchronizationAction.Delete;
            }

            if (file.LocalData.LastModifiedDate > file.RemoteData.LastModifiedDate)
            {
                return AWSSynchronizationAction.Upload;
            }

            return AWSSynchronizationAction.None;
        }
    }
}