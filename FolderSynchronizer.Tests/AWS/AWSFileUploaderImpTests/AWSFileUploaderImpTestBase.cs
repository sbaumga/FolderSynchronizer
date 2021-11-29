using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;

namespace FolderSynchronizer.Tests.AWS.AWSFileUploaderImpTests
{
    [TestFixture]
    public abstract class AWSFileUploaderImpTestBase
    {
        protected Mock<ILogger<AWSFileUploaderImp>> MockLogger { get; set; }

        protected Mock<IAWSPathManager> MockPathManager { get; set; }
        protected Mock<IAWSActionTaker> MockActionTaker { get; set; }
        protected Mock<ILocalFileLister> MockLocalFileLister { get; set; }

        protected string BucketName { get; set; }

        protected AWSFileUploaderImp Uploader { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockLogger = new Mock<ILogger<AWSFileUploaderImp>>();
            MockLogger.Setup(l => l.LogInformation(It.IsAny<string>()));

            MockPathManager = new Mock<IAWSPathManager>(MockBehavior.Strict);

            MockActionTaker = new Mock<IAWSActionTaker>(MockBehavior.Strict);

            MockLocalFileLister = new Mock<ILocalFileLister>(MockBehavior.Strict);

            BucketName = "TestBucket";
            var configData = new ConfigData { BucketName = BucketName };

            Uploader = new AWSFileUploaderImp(MockLogger.Object, MockPathManager.Object, MockActionTaker.Object, MockLocalFileLister.Object, configData);
        }

        protected void SetUpIsPathFile(bool returnValue)
        {
            MockPathManager.Setup(p => p.IsPathFile(It.IsAny<string>())).Returns(returnValue);
        }
    }
}