using Amazon.S3;
using Amazon.S3.Model;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.Extensions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSActionTakerImp : IAWSActionTaker
    {
        private IAWSClientCreator ClientCreator { get; }

        public AWSActionTakerImp(IAWSClientCreator clientCreator)
        {
            ClientCreator = clientCreator ?? throw new ArgumentNullException(nameof(clientCreator));
        }

        public async Task<ListObjectsV2Response> DoListActionAsync(ListObjectsV2Request request)
        {
            return await DoS3Action(client => client.ListObjectsV2Async(request));
        }

        public async Task<PutObjectResponse> DoUploadActionAsync(PutObjectRequest request)
        {
            return await DoS3Action(client => client.PutObjectAsync(request));
        }

        public async Task<DeleteObjectResponse> DoDeleteActionAsync(DeleteObjectRequest request)
        {
            return await DoS3Action(client => client.DeleteObjectAsync(request));
        }

        private async Task<TResponse> DoS3Action<TResponse>(Func<IAmazonS3, Task<TResponse>> s3Action)
        {
            try
            {
                var client = ClientCreator.GetS3Client();
                var response = await s3Action(client);
                return response;
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
                return new AWSException("Check the provided AWS Credentials.", exception);
            }
            else
            {
                return new AWSException("Error occurred: " + exception.Message, exception);
            }
        }
    }
}
