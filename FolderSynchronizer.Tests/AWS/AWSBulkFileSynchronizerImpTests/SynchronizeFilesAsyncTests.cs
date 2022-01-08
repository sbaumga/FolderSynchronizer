using FolderSynchronizer.Data;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSBulkFileSynchronizerImpTests
{
    public class SynchronizeFilesAsyncTests : AWSBulkFileSynchronizerImpTestBase
    {
        [Test]
        public void NoFilesTest()
        {
            SetUpGetSynchronizationStatusForFilesAsync();

            Synchronizer.SynchronizeFilesAsync();

            VerifyNoUploads();
            VerifyNoDeletes();
        }

        private void SetUpGetSynchronizationStatusForFilesAsync(params FileSynchronizationStatusData[] fileStatusData)
        {
            MockSyncChecker.Setup(c => c.GetSynchronizationStatusForFilesAsync()).Returns(Task.FromResult(fileStatusData.AsEnumerable()));
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

            Synchronizer.SynchronizeFilesAsync();

            VerifyNoUploads();
            VerifyNoDeletes();
        }

        private FileSynchronizationStatusData CreateFileSyncStatusData(FileData localData, FileData remoteData)
        {
            var syncData = new FileSynchronizationStatusData { SourceData = localData, DestinationData = remoteData };
            return syncData;
        }

        [Test]
        public void MissingRemoteFile()
        {
            var localData = new FileData { Path = "TestFile" };
            var syncData = CreateFileSyncStatusData(localData, null);

            SetUpFileUploader(localData.Path);
            SetUpGetSynchronizationStatusForFilesAsync(syncData);

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

            Synchronizer.SynchronizeFilesAsync();

            VerifyUpload(localData.Path);
            VerifyNoDeletes();
        }
    }
}