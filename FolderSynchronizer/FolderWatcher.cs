using FolderSynchronizer.AWS;
using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer
{
    public class FolderWatcher
    {
        private string FolderName { get; }

        private IAWSFileUploader FileUploader { get; }
        private IAWSFileDeleter FileDeleter { get; }
        private IAWSFileRenamer FileRenamer { get; }

        private FileSystemWatcher Watcher { get; set; }

        public FolderWatcher(ConfigData configData, IAWSFileUploader fileUploader, IAWSFileDeleter fileDeleter, IAWSFileRenamer fileRenamer)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            FolderName = configData.LocalFolderName;

            FileUploader = fileUploader ?? throw new ArgumentNullException(nameof(fileUploader));
            FileDeleter = fileDeleter ?? throw new ArgumentNullException(nameof(fileDeleter));
            FileRenamer = fileRenamer ?? throw new ArgumentNullException(nameof(fileRenamer));

            InitializeFileWatcher();
        }

        private void InitializeFileWatcher()
        {
            Watcher = new FileSystemWatcher(FolderName);

            Watcher.Created += FileCreated;
            Watcher.Deleted += FileDeleted;
            Watcher.Renamed += FileRenamed;

            Watcher.EnableRaisingEvents = true;
            Watcher.IncludeSubdirectories = true;
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {            
            FileUploader.UploadFileAsync(e.FullPath);
        }

        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            FileDeleter.DeleteRemoteFileFromLocalFileAsync(e.FullPath);
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            var oldLocalPath = e.OldFullPath;
            var newLocalPath = e.FullPath;
            
            FileRenamer.RenameFileAsync(oldLocalPath, newLocalPath);
        }
    }
}
