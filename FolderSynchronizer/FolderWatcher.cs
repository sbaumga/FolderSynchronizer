namespace MusicLibrarySynchronizer
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

        private string GetRemotePath(string localPath)
        {
            return localPath.Replace(FolderName + @"\", "");
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            var remotePath = GetRemotePath(e.FullPath);
            // Upload file
            AWSFileManager.UploadFileAsync(e.FullPath, remotePath);

            // Add message to queue?
        }

        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            var remotePath = GetRemotePath(e.FullPath);
            // Delete file
            AWSFileManager.DeleteFileAsync(remotePath);

            // Add message to queue?
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            var localPath = e.FullPath;
            var oldRemotePath = GetRemotePath(e.OldFullPath);
            var newRemotePath = GetRemotePath(e.FullPath);
            // Rename file
            AWSFileManager.RenameFileAsync(localPath, oldRemotePath, newRemotePath);

            // Add message to queue?
        }
    }
}
