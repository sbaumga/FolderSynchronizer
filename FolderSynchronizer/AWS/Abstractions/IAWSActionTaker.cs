using Amazon.S3;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSActionTaker
    {
        TResponse DoS3Action<TResponse>(Func<AmazonS3Client, TResponse> s3Action);
    }
}