using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;

namespace FolderSynchronizer.Tests.AWS.AWSFileRenamerImpTests
{
    [TestFixture]
    public abstract class AWSFileRenamerImpTestBase
    {
        protected Mock<IAWSFileUploader> MockUploader { get; set; }
        protected Mock<IAWSFileDeleter> MockDeleter { get; set; }
        protected Mock<IAWSPathManager> MockPathManager { get; set; }

        protected AWSFileRenamerImp Renamer { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockUploader = new Mock<IAWSFileUploader>(MockBehavior.Strict);
            MockDeleter = new Mock<IAWSFileDeleter>(MockBehavior.Strict);
            MockPathManager = new Mock<IAWSPathManager>(MockBehavior.Strict);

            Renamer = new AWSFileRenamerImp(MockUploader.Object, MockDeleter.Object, MockPathManager.Object);
        }
    }
}