using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.Implementations;
using Moq;

namespace FolderSynchronizer.Tests.SavedFileListBulkSynchronizerImpTests
{
    public class SynchronizeFilesAsyncTests : BulkFileSynchronizerTestBase<SavedFileListBulkSynchronizerImp, ISavedFileListSyncChecker>
    {
        private Mock<ISavedFileListRecordDeleter> MockSavedFileListRecordDeleter { get; set; }
        private Mock<IAWSPathManager> MockPathManager { get; set; }

        protected override SavedFileListBulkSynchronizerImp CreateBulkSynchronizer(ISavedFileListSyncChecker syncChecker, IAWSFileUploader fileUploader, IAWSFileDeleter fileDeleter, ISynchronizationActionDecider synchronizationActionDecider)
        {
            MockSavedFileListRecordDeleter = new Mock<ISavedFileListRecordDeleter>(MockBehavior.Strict);
            MockPathManager = new Mock<IAWSPathManager>(MockBehavior.Strict);

            return new SavedFileListBulkSynchronizerImp(syncChecker, fileUploader, fileDeleter, synchronizationActionDecider, MockSavedFileListRecordDeleter.Object, MockPathManager.Object);
        }
    }
}