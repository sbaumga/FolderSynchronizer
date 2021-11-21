using Amazon.S3;

namespace FolderSynchronizer.Extensions
{
    public static class AmazonS3ExceptionExtensions
    {
        public static bool IsCredentialException(this AmazonS3Exception exception)
        {
            return exception.ErrorCode != null &&
                    (exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    exception.ErrorCode.Equals("InvalidSecurity"));
        }
    }
}
