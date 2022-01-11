using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Implementations;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.SavedFileListRecordUpdaterImpTests
{
    [TestFixture]
    public class AddOrUpdateRecordAsyncTests
    {
        private Mock<IFileDataCreator> DataCreatorMock { get; set; }
        private Mock<IFileDataListPersister> FileListPersisterMock { get; set; }

        private SavedFileListRecordUpdaterImp Updater { get; set; }

        [SetUp]
        public void SetUp()
        {
            InitializeDataCreatorMock();
            InitializeFileListPersisterMock();

            Updater = new SavedFileListRecordUpdaterImp(DataCreatorMock.Object, FileListPersisterMock.Object);
        }

        private void InitializeDataCreatorMock()
        {
            DataCreatorMock = new Mock<IFileDataCreator>(MockBehavior.Strict);
            DataCreatorMock.Setup(c => c.MakeFileDataFromLocalPath(It.IsAny<string>())).Returns<string>(path => new FileData { Path = path, LastModifiedDate = Date });
        }

        private void InitializeFileListPersisterMock()
        {
            FileListPersisterMock = new Mock<IFileDataListPersister>(MockBehavior.Strict);
            FileListPersisterMock.Setup(p => p.SaveAsync(It.IsAny<IEnumerable<FileData>>())).Returns(Task.CompletedTask);
        }

        [Test]
        public async Task AddTest()
        {
            var testPath = "TestFile.txt";

            FileListPersisterMock.Setup(p => p.LoadAsync()).Returns(Task.FromResult(Enumerable.Empty<FileData>()));

            await RunTest(testPath);
        }

        private async Task RunTest(string testPath)
        {
            await Updater.AddOrUpdateRecordAsync(testPath);

            FileListPersisterMock.Verify(p => p.SaveAsync(It.Is<IEnumerable<FileData>>(e => e.Any(d => d.Path == testPath && d.LastModifiedDate == Date) && e.Count() == 1)));
        }

        private DateTime Date => new(2022, 01, 09);

        [Test]
        public async Task UpdateTest()
        {
            var testPath = "TestFile.txt";

            FileListPersisterMock.Setup(p => p.LoadAsync()).Returns(Task.FromResult(new[] { new FileData { Path = testPath, LastModifiedDate = Date.AddDays(-1) } }.AsEnumerable()));

            await RunTest(testPath);
        }
    }
}