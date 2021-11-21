using Amazon.S3;
using FolderSynchronizer.Extensions;

namespace FolderSynchronizer.AWS
{
    public class AWSActionTaker
    {
        public TResponse DoS3ActionAsync<TResponse>(Func<TResponse> s3Action)
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
