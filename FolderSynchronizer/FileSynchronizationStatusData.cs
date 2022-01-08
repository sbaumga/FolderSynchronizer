using FolderSynchronizer.Data;

namespace FolderSynchronizer
{
    public class FileSynchronizationStatusData
    {
        public FileData? SourceData { get; set; }
        public FileData? DestinationData { get; set; }
    }
}