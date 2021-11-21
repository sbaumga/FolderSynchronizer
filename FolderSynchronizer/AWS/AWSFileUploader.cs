using Amazon.S3.Model;

namespace FolderSynchronizer.AWS
{
    public class AWSFileUploader
    {
        private AWSPathManager PathManager { get; }
        private AWSClientCreator ClientCreator { get; }
        private AWSActionTaker ActionTaker { get; }

        private string BucketName { get; }

        public AWSFileUploader(AWSPathManager pathManager, AWSClientCreator clientCreator, AWSActionTaker actionTaker, ConfigData configData)
        {
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
        }
    }
}
