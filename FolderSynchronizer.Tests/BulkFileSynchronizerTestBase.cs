using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Enums;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests
{
    [TestFixture]
    public abstract class BulkFileSynchronizerTestBase<TBulkFileSynchronizer, TSyncChecker>
        where TBulkFileSynchronizer : IBulkFileSynchronizer
        where TSyncChecker : class, IFileSynchronizationChecker
    {
        protected Mock<TSyncChecker> MockSyncChecker { get; set; }
        protected Mock<IAWSFileUploader> MockFileUploader { get; set; }
        protected Mock<IAWSFileDeleter> MockFileDeleter { get; set; }
        protected Mock<ISynchronizationActionDecider> MockSyncActionDecider { get; set; }

        protected TBulkFileSynchronizer Synchronizer { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockSyncChecker = new Mock<TSyncChecker>(MockBehavior.Strict);

            MockFileUploader = new Mock<IAWSFileUploader>(MockBehavior.Strict);

            MockFileDeleter = new Mock<IAWSFileDeleter>(MockBehavior.Strict);

            MockSyncActionDecider = new Mock<ISynchronizationActionDecider>(MockBehavior.Strict);

            Synchronizer = CreateBulkSynchronizer(MockSyncChecker.Object, MockFileUploader.Object, MockFileDeleter.Object, MockSyncActionDecider.Object);
        }

        protected abstract TBulkFileSynchronizer CreateBulkSynchronizer(TSyncChecker syncChecker, IAWSFileUploader fileUploader, IAWSFileDeleter fileDeleter, ISynchronizationActionDecider synchronizationActionDecider);

        [Test]
        public void NoFilesTest()
        {
            SetUpGetSynchronizationStatusForFilesAsync();

            Synchronizer.SynchronizeFilesAsync();

            VerifyNoAWSActionsTaken();
        }

        private void SetUpGetSynchronizationStatusForFilesAsync(params FileSynchronizationStatusData[] fileStatusData)
        {
            MockSyncChecker.Setup(c => c.GetSynchronizationStatusForFilesAsync()).Returns(Task.FromResult(fileStatusData.AsEnumerable()));
        }

        private void VerifyNoAWSActionsTaken()
        {
            VerifyNoUploads();
            VerifyNoDeletes();
        }

        private void VerifyNoUploads()
        {
            MockFileUploader.Verify(u => u.UploadFileAsync(It.IsAny<string>()), Times.Never);
        }

        private void VerifyNoDeletes()
        {
            MockFileDeleter.Verify(u => u.DeleteRemoteFileAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void RemoteAndLocalFilesMatch()
        {
            var syncData = CreateFileSyncStatusData(new FileData(), new FileData());

            SetUpGetSynchronizationStatusForFilesAsync(syncData);

            SetUpSyncActionDecider(syncData, FileSynchronizationAction.None);

            Synchronizer.SynchronizeFilesAsync();

            VerifyNoAWSActionsTaken();
        }

        private FileSynchronizationStatusData CreateFileSyncStatusData(FileData localData, FileData remoteData)
        {
            var syncData = new FileSynchronizationStatusData { SourceData = localData, DestinationData = remoteData };
            return syncData;
        }

        private void SetUpSyncActionDecider(FileSynchronizationStatusData data, FileSynchronizationAction action)
        {
            MockSyncActionDecider.Setup(d => d.GetNeededActionForFile(data)).Returns(action);
        }

        [Test]
        public void UploadActionWithNoSourceFile()
        {
            var syncData = CreateFileSyncStatusData(null, new FileData());

            SetUpGetSynchronizationStatusForFilesAsync(syncData);

            SetUpSyncActionDecider(syncData, FileSynchronizationAction.Upload);

            Should.Throw<AWSFileSynchronizationException>(() => Synchronizer.SynchronizeFilesAsync());

            VerifyNoAWSActionsTaken();
        }

        [Test]
        public void DeleteActionWithNoDestinationFile()
        {
            var syncData = CreateFileSyncStatusData(new FileData(), null);

            SetUpGetSynchronizationStatusForFilesAsync(syncData);

            SetUpSyncActionDecider(syncData, FileSynchronizationAction.Delete);

            Should.Throw<AWSFileSynchronizationException>(() => Synchronizer.SynchronizeFilesAsync());

            VerifyNoAWSActionsTaken();
        }

        [Test]
        public void MissingRemoteFile()
        {
            var localData = new FileData { Path = "TestFile" };
            var syncData = CreateFileSyncStatusData(localData, null);

            SetUpFileUploader(localData.Path);
            SetUpGetSynchronizationStatusForFilesAsync(syncData);
            SetUpSyncActionDecider(syncData, FileSynchronizationAction.Upload);

            Synchronizer.SynchronizeFilesAsync();

            VerifyUpload(localData.Path);
            VerifyNoDeletes();
        }

        private void SetUpFileUploader(string path)
        {
            MockFileUploader.Setup(u => u.UploadFileAsync(path)).Returns(Task.CompletedTask);
        }

        private void VerifyUpload(string path)
        {
            MockFileUploader.Verify(u => u.UploadFileAsync(path), Times.Once);
        }

        [Test]
        public void MissingLocalFile()
        {
            var remoteData = new FileData { Path = "TestFile" };
            var syncData = CreateFileSyncStatusData(null, remoteData);

            SetUpFileDeleter(remoteData.Path);
            SetUpGetSynchronizationStatusForFilesAsync(syncData);
            SetUpSyncActionDecider(syncData, FileSynchronizationAction.Delete);

            Synchronizer.SynchronizeFilesAsync();

            VerifyNoUploads();
            VerifyDelete(remoteData.Path);
        }

        private void SetUpFileDeleter(string path)
        {
            MockFileDeleter.Setup(u => u.DeleteRemoteFileAsync(path)).Returns(Task.CompletedTask);
        }

        private void VerifyDelete(string path)
        {
            MockFileDeleter.Verify(u => u.DeleteRemoteFileAsync(path), Times.Once);
        }

        [Test]
        public void RemoteFileOutOfDate()
        {
            var localData = new FileData { Path = "TestFile", LastModifiedDate = DateTime.Now };
            var remoteData = new FileData { Path = localData.Path, LastModifiedDate = DateTime.Now.AddDays(-1) };
            var syncData = CreateFileSyncStatusData(localData, remoteData);

            SetUpFileUploader(localData.Path);
            SetUpGetSynchronizationStatusForFilesAsync(syncData);
            SetUpSyncActionDecider(syncData, FileSynchronizationAction.Upload);

            Synchronizer.SynchronizeFilesAsync();

            VerifyUpload(localData.Path);
            VerifyNoDeletes();
        }
    }
}