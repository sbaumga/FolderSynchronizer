using Amazon.S3.Model;

namespace FolderSynchronizer.AWS
{
    public class AWSFileDeleter
    {
        private AWSPathManager PathManager { get; }
        private AWSClientCreator ClientCreator { get; }
        private AWSFileLister FileLister { get; }
        private AWSActionTaker ActionTaker { get; }

        private string BucketName { get; }

        public AWSFileDeleter(AWSPathManager pathManager, AWSClientCreator clientCreator, AWSFileLister fileLister, AWSActionTaker actionTaker, ConfigData configData)
        {
            PathManager = pathManager ?? throw new ArgumentNullException(nameof(pathManager));
            ClientCreator = clientCreator ?? throw new ArgumentNullException(nameof(clientCreator));
            FileLister = fileLister ?? throw new ArgumentNullException(nameof(fileLister));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));

            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }
            BucketName = configData.BucketName;
        }

        public async Task DeleteRemoteFileFromLocalFile(string localPath)
        {
            var remotePath = PathManager.GetRemotePath(localPath);
            await DeleteRemoteFileAsync(remotePath);
        }

        public async Task DeleteRemoteFileAsync(string remotePath)
        {
            if (PathManager.IsPathFile(remotePath))
            {
                await DeleteFile(remotePath);
            }
            else
            {
                await DeleteFolder(remotePath);
            }
        }

        private async Task DeleteFile(string remotePath)
        {
            var client = ClientCreator.GetS3Client();

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
            };

            var response = await ActionTaker.DoS3ActionAsync(async () => await client.DeleteObjectAsync(deleteRequest));
            if (!response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK))
            {
                throw new Exception(response.ToString());
            }
        }

        private async Task DeleteFolder(string remotePath)
        {
            var filesInFolder = await FileLister.ListFilteredFilesAsync(remotePath);

            var deletionTasks = filesInFolder.Select(f => DeleteFile(f));
            await Task.WhenAll(deletionTasks);
        }
    }
}
