using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace MusicLibrarySynchronizer
{
    public class AWSFileManager 
    {
        private string BucketName { get; }
        private BasicAWSCredentials AWSCredentials { get; }

        public AWSFileManager(ConfigData configData)
        {
            BucketName = configData.BucketName;

            AWSCredentials = new BasicAWSCredentials(configData.AccessKey, configData.SecretKey);
        }

        private AmazonS3Client GetS3Client()
        {
            var client = new AmazonS3Client(AWSCredentials, RegionEndpoint.CACentral1);
            return client;
        }

        private TResponse DoS3Action<TResponse>(Func<TResponse> s3Action)
        {
            try
            {
                return s3Action();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }

        public async Task<bool> UploadFileAsync(string localPath, string remotePath)
        {            
            var client = GetS3Client();

            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
                FilePath = localPath,
                ContentType = "text/plain"
            };
            // TODO: use folders
            var response = await DoS3Action(async () => await client.PutObjectAsync(putRequest));
            return response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK);
        }

        public async Task<bool> DeleteFileAsync(string remotePath)
        {
            var client = GetS3Client();

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
            };
            // TODO: Deleting a folder sends a request with the folder name.
            var response = await DoS3Action(async () => await client.DeleteObjectAsync(deleteRequest));
            return response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK);
        }

        public async Task<bool> RenameFileAsync(string localPath, string oldRemotePath, string newRemotePath)
        {
            // Renaming a file in the AWS console says it creates a copy with the new name, then deletes the old one
            // We'll do the same thing here

            var uploadSuccess = await UploadFileAsync(localPath, newRemotePath);
            if (!uploadSuccess)
            {
                // TODO: create a queue for failed requests?
                return false;
            }

            // TODO: better error reporting? What should be reported if we have the new file created, but can't delete the old file?
            var deletionSuccess = await DeleteFileAsync(oldRemotePath);
            return deletionSuccess;
        }
    }
}
