using Amazon.S3;
using Moq;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public abstract class AWSS3ActionTakerImpTestBase<TRequest, TResponse> : AWSActionTakerImpTestBase<IAmazonS3, AmazonS3Exception, TRequest, TResponse>
        where TRequest : class, new()
        where TResponse : class, new()
    {
        protected override void SetupClientCreator(Mock<IAmazonS3> mockAmazon)
        {
            MockClientCreator.Setup(c => c.GetS3Client()).Returns(mockAmazon.Object);
        }

        protected override AmazonS3Exception CreateException()
            => new("TestException");
    }
}