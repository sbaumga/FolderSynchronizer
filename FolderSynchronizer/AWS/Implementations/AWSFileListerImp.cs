using Amazon.S3.Model;
using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileListerImp : IAWSFileLister
    {
        private IAWSActionTaker ActionTaker { get; }

        private string BucketName { get; }

        public AWSFileListerImp(IAWSActionTaker actionTaker, ConfigData configData)
        {
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));

            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            BucketName = configData.BucketName;
        }

        public async Task<IEnumerable<string>> ListFilesAsync()
        {
            var result = await GetAndActOnS3Objects(o => o.Key);
            return result;
        }

        private async Task<IEnumerable<T>> GetAndActOnS3Objects<T>(Func<S3Object, T> resultFunc)
        {
            var s3Objects = await GetS3Objects();
            var result = s3Objects.Select(resultFunc);
            return result;
        }

        private async Task<IList<S3Object>> GetS3Objects()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = BucketName
            };

            var response = await ActionTaker.DoS3Action(async (client) => await client.ListObjectsV2Async(request));
            return response.S3Objects;
        }

        public async Task<IEnumerable<string>> ListFilteredFilesAsync(string startOfPath)
        {
            var result = (await ListFilesAsync()).Where(s => s.StartsWith(startOfPath));
            return result;
        }

        public async Task<IEnumerable<FileData>> GetFileDataAsync()
        {
            var data = await GetAndActOnS3Objects(o => MakeFileDataFroms3Object(o));
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