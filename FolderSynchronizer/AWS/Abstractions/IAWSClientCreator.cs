using Amazon.S3;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSClientCreator
    {
        IAmazonS3 GetS3Client();
    }
}