namespace FolderSynchronizer.AWS
{
    public class AWSFileRenamer
    {
        private AWSFileUploader Uploader { get; }
        private AWSFileDeleter Deleter { get; }

        public AWSFileRenamer(AWSFileUploader uploader, AWSFileDeleter deleter)
        {
            Uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            Deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
        }

        public async Task RenameFileAsync(string localPath, string oldRemotePath, string newRemotePath)
        {
            // Renaming a file in the AWS console says it creates a copy with the new name, then deletes the old one
            // We'll do the same thing here

            // TODO: add support for renaming folders

            // TODO: create a queue for failed requests?
            await Uploader.UploadFileAsync(localPath, newRemotePath);

            // TODO: better error reporting? What should be reported if we have the new file created, but can't delete the old file?
            await Deleter.DeleteFileAsync(oldRemotePath);
        }
    }
}
