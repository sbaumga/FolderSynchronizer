namespace FolderSynchronizer.AWS
{
    public class AWSFileRenamer
    {
        private AWSFileUploader Uploader { get; }
        private AWSFileDeleter Deleter { get; }
        private AWSPathManager PathManager { get; }

        public AWSFileRenamer(AWSFileUploader uploader, AWSFileDeleter deleter, AWSPathManager pathManager)
        {
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
            PathManager = pathManager ?? throw new ArgumentNullException(nameof(pathManager));
        }

        public async Task RenameFileAsync(string localPath, string oldRemotePath, string newRemotePath)
        {
            // Renaming a file in the AWS console says it creates a copy with the new name, then deletes the old one
            // We'll do the same thing here

            if (PathManager.IsPathFile(localPath)) { 
                // TODO: create a queue for failed requests?
                await Uploader.UploadFileAsync(localPath, newRemotePath);
            } else
            {
                await Uploader.UploadFolderAsync(localPath, newRemotePath);
            }

            // TODO: better error reporting? What should be reported if we have the new file created, but can't delete the old file?
            await Deleter.DeleteFileAsync(oldRemotePath);
        }
    }
}
