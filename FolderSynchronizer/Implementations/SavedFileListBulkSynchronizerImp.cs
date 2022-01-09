using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.Implementations
{
    public class SavedFileListBulkSynchronizerImp : BulkFileSynchronizerImpBase<ISavedFileListSyncChecker>, ISavedFileListBulkSynchronizer
    {
        public SavedFileListBulkSynchronizerImp(ISavedFileListSyncChecker fileSyncChecker, IAWSFileUploader uploader, IAWSFileDeleter deleter, ISynchronizationActionDecider synchronizationActionDecider) : base(fileSyncChecker, uploader, deleter, synchronizationActionDecider)
        {
        }
    }
}