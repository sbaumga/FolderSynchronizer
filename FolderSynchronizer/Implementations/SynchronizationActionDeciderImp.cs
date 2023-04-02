using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Enums;

namespace FolderSynchronizer.Implementations
{
    public class SynchronizationActionDeciderImp : ISynchronizationActionDecider
    {
        public FileSynchronizationAction GetNeededActionForFile(FileSynchronizationStatusData file)
        {
            if (file.DestinationData == null)
            {
                return FileSynchronizationAction.Upload;
            }

            if (file.SourceData == null)
            {
                return FileSynchronizationAction.Delete;
            }

            if (file.SourceData.LastModifiedDate > file.DestinationData.LastModifiedDate)
            {
                //return FileSynchronizationAction.Upload;
            }

            return FileSynchronizationAction.None;
        }
    }
}