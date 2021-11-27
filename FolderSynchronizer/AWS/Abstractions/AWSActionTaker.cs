namespace FolderSynchronizer.AWS.Abstractions
{
    public interface AWSActionTaker
    {
        TResponse DoS3Action<TResponse>(Func<TResponse> s3Action);
    }
}