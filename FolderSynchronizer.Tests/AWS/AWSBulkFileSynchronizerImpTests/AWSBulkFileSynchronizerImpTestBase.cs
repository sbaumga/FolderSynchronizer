using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;

namespace FolderSynchronizer.Tests.AWS.AWSBulkFileSynchronizerImpTests
{
    [TestFixture]
    public abstract class AWSBulkFileSynchronizerImpTestBase
    {
        protected Mock<IAWSFileSyncChecker> MockSyncChecker { get; set; }
        protected Mock<IAWSFileUploader> MockFileUploader { get; set; }
        protected Mock<IAWSFileDeleter> MockFileDeleter { get; set; }
        protected Mock<ISynchronizationActionDecider> MockSyncActionDecider { get; set; }

        protected IAWSBulkFileSynchronizer Synchronizer { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockSyncChecker = new Mock<IAWSFileSyncChecker>(MockBehavior.Strict);

            MockFileUploader = new Mock<IAWSFileUploader>(MockBehavior.Strict);

            MockFileDeleter = new Mock<IAWSFileDeleter>(MockBehavior.Strict);

            MockSyncActionDecider = new Mock<ISynchronizationActionDecider>(MockBehavior.Strict);

            Synchronizer = new AWSBulkFileSynchronizerImp(MockSyncChecker.Object, MockFileUploader.Object, MockFileDeleter.Object, MockSyncActionDecider.Object);
        }
    }
}