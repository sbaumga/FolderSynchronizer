using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using FolderSynchronizer.Extensions;

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

        public async Task UploadFileAsync(string localPath, string remotePath)
        {            
            if (!IsPathFile(localPath))
            {
                return;
            }

            remotePath = SanitizeRemotePath(remotePath);

            var client = GetS3Client();

            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
                FilePath = localPath,
                ContentType = "text/plain"
            };

            var listRequest = new ListObjectsV2Request
            {
                BucketName = BucketName
            };

            var listResponse = await DoS3Action(async () => await client.ListObjectsV2Async(listRequest));

            // TODO: use folders
            var response = await DoS3Action(async () => await client.PutObjectAsync(putRequest));
            if (!response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK))
            {
                throw new Exception(response.ToString());
            }
        }

        public async Task DeleteFileAsync(string remotePath)
        {
            if (IsPathFile(remotePath))
            {

            }

            remotePath = SanitizeRemotePath(remotePath);

            var client = GetS3Client();

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
            };
            // TODO: Deleting a folder sends a request with the folder name from the file watcher.
            // Need to handle that here by deleteing all files in that folder
            var response = await DoS3Action(async () => await client.DeleteObjectAsync(deleteRequest));
            if (!response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK))
            {
                throw new Exception(response.ToString());
            }
        }

        public async Task RenameFileAsync(string localPath, string oldRemotePath, string newRemotePath)
        {
            // Renaming a file in the AWS console says it creates a copy with the new name, then deletes the old one
            // We'll do the same thing here

            // TODO: create a queue for failed requests?
            await UploadFileAsync(localPath, newRemotePath);

            // TODO: better error reporting? What should be reported if we have the new file created, but can't delete the old file?
            await DeleteFileAsync(oldRemotePath);
        }

        private string SanitizeRemotePath(string path)
        {
            return path.Replace(@"\", "/");
        }

        private bool IsPathFile(string path)
        {
            return CheckAttributesForPath(path, attributes => !attributes.HasFlag(FileAttributes.Directory));
        }

        private bool CheckAttributesForPath(string path, Func<FileAttributes, bool> attributeCheck)
        {
            var attributes = File.GetAttributes(path);
            return attributeCheck(attributes);
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
                throw GetUnderstandableExceptionFromAmazonS3Exception(amazonS3Exception);
            }
        }

        private Exception GetUnderstandableExceptionFromAmazonS3Exception(AmazonS3Exception exception)
        {
            if (exception.IsCredentialException())
            {
                return new Exception("Check the provided AWS Credentials.");
            }
            else
            {
                return new Exception("Error occurred: " + exception.Message);
            }
        }
    }
}
