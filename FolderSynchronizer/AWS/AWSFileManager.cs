namespace FolderSynchronizer.AWS
{
    public class AWSFileManager 
    {
        private AWSFileUploader Uploader { get; }
        private AWSFileDeleter Deleter { get; }
        private AWSFileRenamer Renamer { get; }

        public AWSFileManager(AWSFileUploader uploader, AWSFileDeleter deleter, AWSFileRenamer renamer)
        {
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
            Renamer = renamer ?? throw new ArgumentNullException(nameof(renamer));
        }

        public async Task UploadFileAsync(string localPath, string remotePath)
        {
            await Uploader.UploadFileAsync(localPath, remotePath);
        }

        public async Task DeleteFileAsync(string remotePath)
        {
            await Deleter.DeleteFileAsync(remotePath);
        }

        public async Task RenameFileAsync(string localPath, string oldRemotePath, string newRemotePath)
        {
            await Renamer.RenameFileAsync(localPath, oldRemotePath, newRemotePath);
        }
    }
}
