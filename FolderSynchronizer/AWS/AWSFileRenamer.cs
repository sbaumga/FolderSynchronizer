using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS
{
    public class AWSFileRenamer
    {
        private IAWSFileUploader Uploader { get; }
        private AWSFileDeleter Deleter { get; }
        private IAWSPathManager PathManager { get; }

        public AWSFileRenamer(IAWSFileUploader uploader, AWSFileDeleter deleter, IAWSPathManager pathManager)
        {
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
            PathManager = pathManager ?? throw new ArgumentNullException(nameof(pathManager));
        }

        public async Task RenameFileAsync(string oldLocalPath, string newLocalPath)
        {
            // Renaming a file in the AWS console says it creates a copy with the new name, then deletes the old one
            // We'll do the same thing here
            await UploadRenamedFile(newLocalPath);

            await DeleteRenamedFile(oldLocalPath);
        }

        private async Task UploadRenamedFile(string localPath)
        {
            if (PathManager.IsPathFile(localPath))
            {
                // TODO: create a queue for failed requests?
                await Uploader.UploadFileAsync(localPath);
            }
            else
            {
                await Uploader.UploadFolderAsync(localPath);
            }
        }

        private async Task DeleteRenamedFile(string oldLocalPath)
        {
            var oldRemotePath = PathManager.GetRemotePath(oldLocalPath);
            // TODO: better error reporting? What should be reported if we have the new file created, but can't delete the old file?
            await Deleter.DeleteRemoteFileAsync(oldRemotePath);
        }
    }
}
