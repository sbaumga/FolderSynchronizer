using Amazon.S3.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileUploaderImp : IAWSFileUploader
    {
        private FolderSynchronizer.Abstractions.ILogger Logger { get; }

        private IAWSPathManager PathManager { get; }
        private IAWSActionTaker ActionTaker { get; }
        private ILocalFileLister LocalFileLister { get; }

        private string BucketName { get; }

        public AWSFileUploaderImp(FolderSynchronizer.Abstractions.ILogger<AWSFileUploaderImp> logger, IAWSPathManager pathManager, IAWSActionTaker actionTaker, ILocalFileLister localFileLister, ConfigData configData)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            PathManager = pathManager ?? throw new ArgumentNullException(nameof(pathManager));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));
            LocalFileLister = localFileLister ?? throw new ArgumentNullException();

            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }
            BucketName = configData.BucketName;
        }

        public async Task UploadFileAsync(string localPath)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            if (!PathManager.IsPathFile(localPath))
            {
                return;
            }

            var remotePath = PathManager.GetRemotePath(localPath);

            LogUploadFileMessage(localPath, remotePath);

            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
                FilePath = localPath,
                // TODO: make this change based on file type
                ContentType = "text/plain"
            };

            var response = ActionTaker.DoUploadAction(putRequest);
            if (!response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK))
            {
                throw new AWSFileUploadException($"Upload of file \"{localPath}\" failed: {response}");
            }

            LogUploadFileCompleteMessage(localPath, remotePath);
        }

        private void LogUploadFileMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"Uploading file {localPath} to {remotePath}");
        }

        private void LogUploadFileCompleteMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"File {localPath} successfully uploaded to {remotePath}");
        }

        public async Task UploadFolderAsync(string localPath)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            var remotePath = PathManager.GetRemotePath(localPath);
            LogUploadFolderMessage(localPath, remotePath);

            var files = LocalFileLister.GetFilePathsForFolder(localPath);
            var uploadTasks = files.Select(f =>
            {
                return UploadFileAsync(f);
            });
            await Task.WhenAll(uploadTasks);

            LogUploadFolderCompleteMessage(localPath, remotePath);
        }

        private void LogUploadFolderMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"Uploading folder {localPath} to {remotePath}");
        }

        private void LogUploadFolderCompleteMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"Folder {localPath} successfully uploaded to {remotePath}");
        }
    }
}