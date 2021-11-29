using Amazon.S3;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    [TestFixture]
    public abstract class AWSActionTakerImpTestBase
    {
        protected Mock<IAmazonS3> MockAmazonS3 { get; set; }
        protected Mock<IAWSClientCreator> MockClientCreator { get; set; }
        protected AWSActionTakerImp ActionTaker { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockAmazonS3 = new Mock<IAmazonS3>(MockBehavior.Strict);

            MockClientCreator = new Mock<IAWSClientCreator>(MockBehavior.Strict);
            MockClientCreator.Setup(c => c.GetS3Client()).Returns(MockAmazonS3.Object);

            ActionTaker = new AWSActionTakerImp(MockClientCreator.Object);
        }

        protected AmazonS3Exception CreateS3Exception(string? errorCode)
        {
            var exception = new AmazonS3Exception("TestException")
            {
                ErrorCode = errorCode
            };
            return exception;
        }
    }
}