using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using FolderSynchronizer.Data;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileSyncCheckerImpTests
{
    public class GetSynchronizationStatusForFilesAsyncTests : GetSynchronizationStatusForFilesAsyncTestBase<AWSFileSyncCheckerImp>
    {
        protected string LocalFolder => "TestFolder";

        protected Mock<ILocalFileLister> LocalFileListerMock { get; set; }
        protected Mock<IAWSFileLister> AWSFileListerMock { get; set; }
        protected Mock<IAWSPathManager> PathManagerMock { get; set; }

        protected override void InitializeFileSyncChecker()
        {
            var configData = new LocalConfigData { LocalFolderName = LocalFolder };

            LocalFileListerMock = new Mock<ILocalFileLister>(MockBehavior.Strict);
            AWSFileListerMock = new Mock<IAWSFileLister>(MockBehavior.Strict);
            PathManagerMock = new Mock<IAWSPathManager>(MockBehavior.Strict);

            FileSyncChecker = new AWSFileSyncCheckerImp(configData, LocalFileListerMock.Object, AWSFileListerMock.Object, PathManagerMock.Object);
        }

        protected override void SetUpSourceFileData(params FileData[] data)
        {
            LocalFileListerMock.Setup(x => x.GetFileDataForFolder(LocalFolder)).Returns(data);
        }

        protected override void SetUpDestinationFileData(params FileData[] data)
        {
            AWSFileListerMock.Setup(x => x.GetFileDataAsync()).Returns(Task.FromResult(data.AsEnumerable()));
        }

        protected override void SetUpGetDestinationPath(string sourcePath, string destinationPath)
        {
            PathManagerMock.Setup(p => p.GetRemotePath(sourcePath)).Returns(destinationPath);
        }
    }
}