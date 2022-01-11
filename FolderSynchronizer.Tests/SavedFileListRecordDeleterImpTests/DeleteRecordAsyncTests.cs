using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Implementations;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.SavedFileListRecordDeleterImpTests
{
    [TestFixture]
    public class DeleteRecordAsyncTests
    {
        private Mock<IFileDataListPersister> FileListPersisterMock { get; set; }

        private SavedFileListRecordDeleterImp Deleter { get; set; }

        [SetUp]
        public void SetUp()
        {
            FileListPersisterMock = new Mock<IFileDataListPersister>(MockBehavior.Strict);
            FileListPersisterMock.Setup(p => p.SaveAsync(It.IsAny<IEnumerable<FileData>>())).Returns(Task.CompletedTask);

            Deleter = new SavedFileListRecordDeleterImp(FileListPersisterMock.Object);
        }

        [Test]
        public async Task FileNotPresentTest()
        {
            SetUpPersister();

            await Deleter.DeleteRecordAsync("TestFile.txt");
        }

        private void SetUpPersister(params FileData[] data)
        {
            FileListPersisterMock.Setup(p => p.LoadAsync()).Returns(Task.FromResult(data.AsEnumerable()));
        }

        [Test]
        public async Task SuccessTest()
        {
            var pathToDelete = "TestFile.txt";
            var pathToKeep = "TestFile2.txt";

            SetUpPersister(new FileData { Path = pathToDelete }, new FileData { Path = pathToKeep });

            await Deleter.DeleteRecordAsync(pathToDelete);

            FileListPersisterMock.Verify(p => p.SaveAsync(It.Is<IEnumerable<FileData>>(e => e.Count() == 1 && e.Any(d => d.Path == pathToKeep))));
        }
    }
}