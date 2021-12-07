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

        [Obsolete]
        public TResponse DoS3Action<TResponse>(Func<IAmazonS3, TResponse> s3Action)
        {
            return DoS3ActionInternal(s3Action);
        }

        private TResponse DoS3ActionInternal<TResponse>(Func<IAmazonS3, TResponse> s3Action)
        {
            try
            {
                var client = ClientCreator.GetS3Client();
                var response = s3Action(client);
                return response;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                throw GetUnderstandableExceptionFromAmazonS3Exception(amazonS3Exception);
            }
        }

        public PutObjectResponse DoUploadAction(PutObjectRequest request)
        {
            return DoS3ActionInternal((client) => client.PutObjectAsync(request).Result);
        }

        public DeleteObjectResponse DoDeleteAction(DeleteObjectRequest request)
        {
            return DoS3ActionInternal((client) => client.DeleteObjectAsync(request).Result);
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
