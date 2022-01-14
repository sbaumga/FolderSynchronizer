using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class SavedFileListBulkSynchronizerImp : BulkFileSynchronizerImpBase<ISavedFileListSyncChecker>, ISavedFileListBulkSynchronizer
    {
        private ISavedFileListRecordDeleter SavedFileListRecordDeleter { get; }

        public SavedFileListBulkSynchronizerImp(ISavedFileListSyncChecker fileSyncChecker, IAWSFileUploader uploader, IAWSFileDeleter deleter, ISynchronizationActionDecider synchronizationActionDecider, ISavedFileListRecordDeleter savedFileListRecordDeleter) : base(fileSyncChecker, uploader, deleter, synchronizationActionDecider)
        {
            SavedFileListRecordDeleter = savedFileListRecordDeleter ?? throw new ArgumentException(nameof(savedFileListRecordDeleter));
        }

        protected override async Task DeleteFile(FileData file)
        {
            await base.DeleteFile(file);

            await SavedFileListRecordDeleter.DeleteRecordAsync(file.Path);
        }
    }
}