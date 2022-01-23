using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class SavedFileListBulkSynchronizerImp : BulkFileSynchronizerImpBase<ISavedFileListSyncChecker>, ISavedFileListBulkSynchronizer
    {
        private ISavedFileListRecordDeleter SavedFileListRecordDeleter { get; }
        private IAWSPathManager AWSPathManager { get; }

        public SavedFileListBulkSynchronizerImp(ISavedFileListSyncChecker fileSyncChecker, IAWSFileUploader uploader, IAWSFileDeleter deleter, ISynchronizationActionDecider synchronizationActionDecider, ISavedFileListRecordDeleter savedFileListRecordDeleter, IAWSPathManager awsPathManager) : base(fileSyncChecker, uploader, deleter, synchronizationActionDecider)
        {
            SavedFileListRecordDeleter = savedFileListRecordDeleter ?? throw new ArgumentException(nameof(savedFileListRecordDeleter));
            AWSPathManager = awsPathManager ?? throw new ArgumentException(nameof(awsPathManager));
        }

        protected override async Task DeleteFile(FileData file)
        {
            var localPath = file.Path;
            file.Path = AWSPathManager.GetRemotePath(localPath);

            await base.DeleteFile(file);

            await SavedFileListRecordDeleter.DeleteRecordAsync(localPath);
        }
    }
}