using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
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
                throw GetUnderstandableExceptionFromS3Exception(amazonS3Exception);
            }
        }

        public async Task<PutObjectResponse> DoUploadActionAsync(PutObjectRequest request)
        {
            return await DoS3ActionOnObject(client => client.PutObjectAsync(request), request.Key);
        }

        public async Task<GetObjectResponse> DoGetObjectAsync(GetObjectRequest request)
        {
            return await DoS3ActionOnObject(client => client.GetObjectAsync(request), request.Key);
        }

        public async Task<DeleteObjectResponse> DoDeleteActionAsync(DeleteObjectRequest request)
        {
            return await DoS3ActionOnObject(client => client.DeleteObjectAsync(request), request.Key);
        }

        private async Task<TResponse> DoS3ActionOnObject<TResponse>(Func<IAmazonS3, Task<TResponse>> s3Action, string objectKey)
        {
            try
            {
                var client = ClientCreator.GetS3Client();
                var response = await s3Action(client);
                return response;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode == "NoSuchKey")
                {
                    throw new FileDoesNotExistInBucketException(objectKey);
                }

                throw GetUnderstandableExceptionFromS3Exception(amazonS3Exception);
            }
        }

        private Exception GetUnderstandableExceptionFromS3Exception(AmazonS3Exception exception)
        {
            return GetUnderstandableExceptionFromAmazonServiceException(exception);
        }

        private Exception GetUnderstandableExceptionFromAmazonServiceException(AmazonServiceException exception)
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

        public async Task<ReceiveMessageResponse> DoSQSPollingAsync(ReceiveMessageRequest request)
        {
            return await DoSQSAction(client => client.ReceiveMessageAsync(request));
        }

        public async Task<DeleteMessageResponse> DoSQSMessageDeletionAsync(DeleteMessageRequest request)
        {
            return await DoSQSAction(client => client.DeleteMessageAsync(request));
        }

        private async Task<TResponse> DoSQSAction<TResponse>(Func<IAmazonSQS, Task<TResponse>> sqsAction)
        {
            try
            {
                var client = ClientCreator.GetSQSClient();
                var response = await sqsAction(client);
                return response;
            }
            catch (AmazonSQSException amazonSQSException)
            {
                throw GetUnderstandableExceptionFromAmazonServiceException(amazonSQSException);
            }
        }
    }
}
