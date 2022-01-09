using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.Implementations;

namespace FolderSynchronizer.Tests.SavedFileListBulkSynchronizerImpTests
{
    public class SynchronizeFilesAsyncTests : BulkFileSynchronizerTestBase<SavedFileListBulkSynchronizerImp, ISavedFileListSyncChecker>
    {
        protected override SavedFileListBulkSynchronizerImp CreateBulkSynchronizer(ISavedFileListSyncChecker syncChecker, IAWSFileUploader fileUploader, IAWSFileDeleter fileDeleter, ISynchronizationActionDecider synchronizationActionDecider)
        {
            return new SavedFileListBulkSynchronizerImp(syncChecker, fileUploader, fileDeleter, synchronizationActionDecider);
        }
    }
}