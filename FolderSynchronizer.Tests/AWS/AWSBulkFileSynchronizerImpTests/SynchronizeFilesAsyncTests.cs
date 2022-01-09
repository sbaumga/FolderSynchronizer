using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;

namespace FolderSynchronizer.Tests.AWS.AWSBulkFileSynchronizerImpTests
{
    public class SynchronizeFilesAsyncTests : BulkFileSynchronizerTestBase<AWSBulkFileSynchronizerImp, IAWSFileSyncChecker>
    {
        protected override AWSBulkFileSynchronizerImp CreateBulkSynchronizer(IAWSFileSyncChecker syncChecker, IAWSFileUploader fileUploader, IAWSFileDeleter fileDeleter, ISynchronizationActionDecider synchronizationActionDecider)
        {
            return new AWSBulkFileSynchronizerImp(syncChecker, fileUploader, fileDeleter, synchronizationActionDecider);
        }
    }
}