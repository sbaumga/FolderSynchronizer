using Amazon.S3;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.Extensions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSActionTakerImp : AWSActionTaker
    {
        public TResponse DoS3Action<TResponse>(Func<TResponse> s3Action)
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
                return new AWSException("Check the provided AWS Credentials.", exception);
            }
            else
            {
                return new AWSException("Error occurred: " + exception.Message, exception);
            }
        }
    }
}
