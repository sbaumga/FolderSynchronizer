using Amazon.S3.Model;
using FolderSynchronizer.AWS.Implementations;

namespace FolderSynchronizer.AWS
{
    public class AWSFileLister
    {
        private AWSClientCreator ClientCreator { get; }
        private AWSActionTakerImp ActionTaker { get; }

        private string BucketName { get; }

        public AWSFileLister(AWSClientCreator clientCreator, AWSActionTakerImp actionTaker, ConfigData configData)
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
            var s3Objects = await GetS3Objects();
            var result = s3Objects.Select(x => x.Key);
            return result;
        }

        private async Task<IList<S3Object>> GetS3Objects()
        {
            var client = ClientCreator.GetS3Client();

            var request = new ListObjectsV2Request
            {
                BucketName = BucketName
            };

            var response = await ActionTaker.DoS3Action(async () => await client.ListObjectsV2Async(request));
            return response.S3Objects;
        }

        public async Task<IEnumerable<string>> ListFilteredFilesAsync(string startOfPath)
        {
            var result = (await ListFilesAsync()).Where(s => s.StartsWith(startOfPath));
            return result;
        }

        public async Task<IEnumerable<FileData>> GetFileDataAsync()
        {
            var s3Objects = await GetS3Objects();
            var data = s3Objects.Select(o => MakeFileDataFroms3Object(o));
            return data;
        }

        private FileData MakeFileDataFroms3Object(S3Object s3Object)
        {
            var data = new FileData
            {
                Path = s3Object.Key,
                LastModifiedDate = s3Object.LastModified.ToUniversalTime(),
            };
            return data;
        }
    }
}
