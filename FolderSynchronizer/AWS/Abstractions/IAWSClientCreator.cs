using Amazon.S3;
using Amazon.SQS;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSClientCreator
    {
        IAmazonS3 GetS3Client();
        IAmazonSQS GetSqsClient();
    }
}