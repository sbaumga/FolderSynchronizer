using Amazon.S3;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSClientCreator
    {
        AmazonS3Client GetS3Client();
    }
}