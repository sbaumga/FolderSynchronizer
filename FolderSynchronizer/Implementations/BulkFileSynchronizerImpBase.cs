using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Enums;

namespace FolderSynchronizer.Implementations
{
    public abstract class BulkFileSynchronizerImpBase<TFileSyncChecker>
        where TFileSyncChecker : IFileSynchronizationChecker
    {
        private TFileSyncChecker FileSyncChecker { get; }
        private IAWSFileUploader Uploader { get; }
        private IAWSFileDeleter Deleter { get; }
        private ISynchronizationActionDecider SyncActionDecider { get; }

        protected BulkFileSynchronizerImpBase(
            TFileSyncChecker fileSyncChecker,
            IAWSFileUploader uploader,
            IAWSFileDeleter deleter,
            ISynchronizationActionDecider synchronizationActionDecider)
        {
            FileSyncChecker = fileSyncChecker ?? throw new ArgumentNullException(nameof(fileSyncChecker));
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
                    await UploadFile(file.SourceData);
                    break;

                case FileSynchronizationAction.Delete:
                    await DeleteFile(file.DestinationData);
                    break;
            }
        }

        private async Task UploadFile(FileData file)
        {
            if (file == null)
            {
                throw new AWSFileSynchronizationException($"An {FileSynchronizationAction.Upload} action was returned with no source file to upload");
            }

            await Uploader.UploadFileAsync(file.Path);
        }

        protected virtual async Task DeleteFile(FileData file)
        {
            if (file == null)
            {
                throw new AWSFileSynchronizationException($"An {FileSynchronizationAction.Delete} action was returned with no destination file to delete");
            }

            await Deleter.DeleteRemoteFileAsync(file.Path);
        }
    }
}
