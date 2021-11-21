using Amazon.S3.Model;

namespace FolderSynchronizer.AWS
{
    public class AWSFileLister
    {
        private AWSClientCreator ClientCreator { get; }
        private AWSActionTaker ActionTaker { get; }

        private string BucketName { get; }

        public AWSFileLister(AWSClientCreator clientCreator, AWSActionTaker actionTaker, ConfigData configData)
        {
            ClientCreator = clientCreator ?? throw new ArgumentNullException(nameof(clientCreator));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof (actionTaker));

            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            BucketName = configData.BucketName;
        }

        public async Task<IEnumerable<string>> ListFilesAsync()
        {
            var client = ClientCreator.GetS3Client();

            var request = new ListObjectsV2Request
            {
                BucketName = BucketName
            };

            var response = await ActionTaker.DoS3ActionAsync(async () => await client.ListObjectsV2Async(request));
            var result = response.S3Objects.Select(x => x.Key);
            return result;
        }

        public async Task<IEnumerable<string>> ListFilteredFilesAsync(string startOfPath)
        {
            var result = (await ListFilesAsync()).Where(s => s.StartsWith(startOfPath));
            return result;
        }
    }
}
