using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Implementations;
using Moq;
using System.IO;

namespace FolderSynchronizer.Tests.FileDataListPersisterImpTests
{
    public abstract class FileDataListPersisterImpTestBase : LocalFolderTestBase
    {
        protected Mock<ISerializer> MockSerializer { get; set; }

        protected FileDataListPersisterImp FileDataListPersister { get; set; }

        protected string FilePath => Path.Combine(GetTestFolderPath(), "TestFile.json");

        public override void SetUp()
        {
            base.SetUp();

            MockSerializer = new Mock<ISerializer>(MockBehavior.Strict);

            var configData = new LocalConfigData { LocalFileListSaveFileLocation = FilePath };
            FileDataListPersister = new FileDataListPersisterImp(configData, MockSerializer.Object);
        }
    }
}