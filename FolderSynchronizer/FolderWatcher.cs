using FolderSynchronizer.AWS;

namespace FolderSynchronizer
{
    public class FolderWatcher
    {
        private FileSystemWatcher Watcher { get; }

        private AWSFileManager AWSFileManager { get; }

        private string FolderName { get; }

        public FolderWatcher(ConfigData config, AWSFileManager awsFileManager)
        {
            FolderName = config.LocalFolderName;

            Watcher = new FileSystemWatcher(FolderName);

            Watcher.Created += FileCreated;
            Watcher.Deleted += FileDeleted;
            Watcher.Renamed += FileRenamed;

            Watcher.EnableRaisingEvents = true;
            Watcher.IncludeSubdirectories = true;

            AWSFileManager = awsFileManager;
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {            
            AWSFileManager.UploadFileAsync(e.FullPath);

            // Add message to queue?
        }

        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            AWSFileManager.DeleteRemoteFileFromLocalFileAsync(e.FullPath);

            // Add message to queue?
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            var oldLocalPath = e.OldFullPath;
            var newLocalPath = e.FullPath;
            
            AWSFileManager.RenameFileAsync(oldLocalPath, newLocalPath);

            // Add message to queue?
        }
    }
}
