using Amazon.S3;
using Amazon.S3.Model;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSActionTaker
    {
        [Obsolete]
        TResponse DoS3Action<TResponse>(Func<IAmazonS3, TResponse> s3Action);

        PutObjectResponse DoUploadAction(PutObjectRequest request);
        DeleteObjectResponse DoDeleteAction(DeleteObjectRequest request);
    }
}