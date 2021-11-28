using Amazon.S3.Model;
using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS
{
    public class AWSFileUploader
    {
        private ILogger Logger { get; }

        private IAWSPathManager PathManager { get; }
        private IAWSActionTaker ActionTaker { get; }
        private LocalFileLister LocalFileLister { get; }

        private string BucketName { get; }

        public AWSFileUploader(ILogger<AWSFileUploader> logger, IAWSPathManager pathManager, IAWSActionTaker actionTaker, LocalFileLister localFileLister, ConfigData configData)
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
                ContentType = "text/plain"
            };

            var response = await ActionTaker.DoS3Action(async (client) => await client.PutObjectAsync(putRequest));
            if (!response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK))
            {
                throw new Exception(response.ToString());
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