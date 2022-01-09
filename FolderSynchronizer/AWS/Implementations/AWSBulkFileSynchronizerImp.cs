using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.Implementations;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSBulkFileSynchronizerImp : BulkFileSynchronizerImpBase<IAWSFileSyncChecker>, IAWSBulkFileSynchronizer
    {
        public AWSBulkFileSynchronizerImp(IAWSFileSyncChecker fileSyncChecker, IAWSFileUploader uploader, IAWSFileDeleter deleter, ISynchronizationActionDecider synchronizationActionDecider) : base(fileSyncChecker, uploader, deleter, synchronizationActionDecider)
        {
        }
    }
}