using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS
{
    public class AWSFileManager 
    {
        private IAWSFileUploader Uploader { get; }
        private AWSFileDeleter Deleter { get; }
        private AWSFileRenamer Renamer { get; }

        public AWSFileManager(IAWSFileUploader uploader, AWSFileDeleter deleter, AWSFileRenamer renamer)
        {
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
            Renamer = renamer ?? throw new ArgumentNullException(nameof(renamer));
        }

        public async Task UploadFileAsync(string localPath)
        {
            await Uploader.UploadFileAsync(localPath);
        }

        public async Task DeleteRemoteFileFromLocalFileAsync(string localPath)
        {
            await Deleter.DeleteRemoteFileAsync(localPath);
        }

        public async Task RenameFileAsync(string oldLocalPath, string newLocalPath)
        {
            await Renamer.RenameFileAsync(oldLocalPath, newLocalPath);
        }
    }
}
