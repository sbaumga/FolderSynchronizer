﻿using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileRenamerImp : IAWSFileRenamer
    {
        private IAWSFileUploader Uploader { get; }
        private IAWSFileDeleter Deleter { get; }
        private IAWSPathManager PathManager { get; }

        public AWSFileRenamerImp(IAWSFileUploader uploader, IAWSFileDeleter deleter, IAWSPathManager pathManager)
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
                await Uploader.UploadFileAsync(localPath);
            }
            else
            {
                await Uploader.UploadFolderAsync(localPath);
            }
        }

        private async Task DeleteRenamedFile(string oldLocalPath)
        {
            // TODO: better error reporting? What should be reported if we have the new file created, but can't delete the old file?
            await Deleter.DeleteRemoteFileFromLocalFileAsync(oldLocalPath);
        }
    }
}
