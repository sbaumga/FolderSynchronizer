namespace FolderSynchronizer.Data
{
    public class LocalConfigData
    {
        public string LocalFolderName { get; set; }
        public string LocalFileListSaveFileLocation { get; set; }
        public bool PeriodicQueuePollingOn { get; set; }
        public int ExecutionIntervalSeconds { get; set; }
    }
}