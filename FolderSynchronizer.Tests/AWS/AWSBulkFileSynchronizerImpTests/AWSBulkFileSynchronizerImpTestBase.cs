using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSBulkFileSynchronizerImpTests
{
    [TestFixture]
    public abstract class AWSBulkFileSynchronizerImpTestBase
    {
        protected Mock<IAWSFileSyncChecker> MockSyncChecker { get; set; }
        protected Mock<IAWSFileUploader> MockFileUploader { get; set; }
        protected Mock<IAWSFileDeleter> MockFileDeleter { get; set; }

        protected IAWSBulkFileSynchronizer Synchronizer { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockSyncChecker = new Mock<IAWSFileSyncChecker>(MockBehavior.Strict);

            MockFileUploader = new Mock<IAWSFileUploader>(MockBehavior.Strict);

            MockFileDeleter = new Mock<IAWSFileDeleter>(MockBehavior.Strict);

            Synchronizer = new AWSBulkFileSynchronizerImp(MockSyncChecker.Object, MockFileUploader.Object, MockFileDeleter.Object);
        }
    }
}
