using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Implementations;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.SavedFileListSyncCheckerImpTests
{
    public class GetSynchronizationStatusForFilesAsyncTests : GetSynchronizationStatusForFilesAsyncTestBase<SavedFileListSyncCheckerImp>
    {
        private string LocalFolder => "TestFolder";
        private string SaveFilePath = "TestFile.txt";

        private Mock<ILocalFileLister> LocalFileListerMock { get; set; }
        private Mock<IFileDataListPersister> FileDataListPersisterMock { get; set; }

        protected override void InitializeFileSyncChecker()
        {
            var configData = new LocalConfigData { LocalFolderName = LocalFolder, LocalFileListSaveFileLocation = SaveFilePath };

            LocalFileListerMock = new Mock<ILocalFileLister>(MockBehavior.Strict);
            FileDataListPersisterMock = new Mock<IFileDataListPersister>(MockBehavior.Strict);

            FileSyncChecker = new SavedFileListSyncCheckerImp(configData, LocalFileListerMock.Object, FileDataListPersisterMock.Object);
        }

        protected override void SetUpSourceFileData(params FileData[] data)
        {
            LocalFileListerMock.Setup(l => l.GetFileDataForFolder(LocalFolder)).Returns(data);
        }

        protected override void SetUpDestinationFileData(params FileData[] data)
        {
            FileDataListPersisterMock.Setup(p => p.LoadAsync()).Returns(Task.FromResult(data.AsEnumerable()));
        }

        protected override void SetUpGetDestinationPath(string sourcePath, string destinationPath)
        {
        }
    }
}