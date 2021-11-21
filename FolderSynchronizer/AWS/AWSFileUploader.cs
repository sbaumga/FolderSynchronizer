using Amazon.S3.Model;

namespace FolderSynchronizer.AWS
{
    public class AWSFileUploader
    {
        private ILogger Logger { get; }

        private AWSPathManager PathManager { get; }
        private AWSClientCreator ClientCreator { get; }
        private AWSActionTaker ActionTaker { get; }

        private string BucketName { get; }

        public AWSFileUploader(ILogger<AWSFileUploader> logger, AWSPathManager pathManager, AWSClientCreator clientCreator, AWSActionTaker actionTaker, ConfigData configData)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            PathManager = pathManager ?? throw new ArgumentNullException(nameof(pathManager));
            ClientCreator = clientCreator ?? throw new ArgumentNullException(nameof(clientCreator));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));

            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }
            BucketName = configData.BucketName;
        }

        public async Task UploadFileAsync(string localPath, string remotePath)
        {
            if (!PathManager.IsPathFile(localPath))
            {
                return;
            }

            remotePath = PathManager.SanitizeRemotePath(remotePath);

            LogUploadFileMessage(localPath, remotePath);

            var client = ClientCreator.GetS3Client();

            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
                FilePath = localPath,
                ContentType = "text/plain"
            };

            var response = await ActionTaker.DoS3ActionAsync(async () => await client.PutObjectAsync(putRequest));
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

        public async Task UploadFolderAsync(string localPath, string remotePath)
        {
            LogUploadFolderMessage(localPath, remotePath);

            var files = PathManager.GetFilePathsForFolder(localPath);
            var uploadTasks = files.Select(f =>
            {
                var fileRemotePath = PathManager.SanitizeRemotePath(f.Replace(localPath, remotePath));
                return UploadFileAsync(f, fileRemotePath);
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
