using Amazon.Runtime;

namespace FolderSynchronizer.Extensions
{
    public static class AmazonServiceExceptionExtensions
    {
        public static bool IsCredentialException(this AmazonServiceException exception)
        {
            return exception.ErrorCode != null &&
                    (exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    exception.ErrorCode.Equals("InvalidSecurity"));
        }
    }
}