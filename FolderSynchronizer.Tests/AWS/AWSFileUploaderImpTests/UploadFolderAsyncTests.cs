using Amazon.S3.Model;
using FolderSynchronizer.AWS.Exceptions;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileUploaderImpTests
{
    public class UploadFolderAsyncTests : AWSFileUploaderImpTestBase
    {
        [Test]
        public async Task NullPathTest()
        {
            Should.Throw<ArgumentNullException>(async () => await Uploader.UploadFolderAsync(null));
        }

        [Test]
        public async Task EmptyPathTest()
        {
            Should.Throw<ArgumentNullException>(async () => await Uploader.UploadFolderAsync(string.Empty));
        }

        [Test]
        public async Task EmptyFolderTest()
        {
            const string path = "Garbage";
            SetUpRemotePath(path, path);
            SetUpGetFilePathsForFolder(path);

            await Uploader.UploadFolderAsync(path);

            MockActionTaker.Verify(a => a.DoUploadActionAsync(It.IsAny<PutObjectRequest>()), Times.Never);
        }

        [Test]
        public async Task SingleFileFolderTest_SuccessfulUpload()
        {
            const string folderPath = "Garbage";
            SetUpSingleFileTest(folderPath, HttpStatusCode.OK);

            await Uploader.UploadFolderAsync(folderPath);

            MockActionTaker.Verify(a => a.DoUploadActionAsync(It.IsAny<PutObjectRequest>()), Times.Once);
        }

        private void SetUpSingleFileTest(string folderPath, HttpStatusCode reponseStatusCode)
        {
            SetUpRemotePath(folderPath, folderPath);

            const string filePath = "Garbage/Trash.txt";
            SetUpFile(filePath, filePath, reponseStatusCode);

            SetUpGetFilePathsForFolder(folderPath, filePath);
        }

        private void SetUpFile(string localPath, string remotePath, HttpStatusCode responseStatus)
        {
            SetUpRemotePath(localPath, remotePath);
            SetUpIsPathFile(true, remotePath);
            SetUpUploadAction(responseStatus, localPath, remotePath);

            if (responseStatus == HttpStatusCode.OK)
            {
                SetUpSavedFileListRecordUpdater(localPath);
            }
        }

        [Test]
        public async Task SingleFileFolderTest_FailedUpload()
        {
            const string folderPath = "Garbage";
            SetUpSingleFileTest(folderPath, HttpStatusCode.BadRequest);

            Should.Throw<AWSFileUploadException>(async () => await Uploader.UploadFolderAsync(folderPath));

            MockActionTaker.Verify(a => a.DoUploadActionAsync(It.IsAny<PutObjectRequest>()), Times.Once);
            VerifySavedFileListNotUpdated();
        }

        [Test]
        public async Task MultipleFileFolderTest_AllUploadsSucceed()
        {
            const string folderPath = "Garbage";
            SetUpRemotePath(folderPath, folderPath);

            const string filePath1 = "Garbage/Trash.txt";
            SetUpFile(filePath1, filePath1, HttpStatusCode.OK);

            const string filePath2 = "Garbage/Rubbish.txt";
            SetUpFile(filePath2, filePath2, HttpStatusCode.OK);

            SetUpGetFilePathsForFolder(folderPath, filePath1, filePath2);

            await Uploader.UploadFolderAsync(folderPath);

            MockActionTaker.Verify(a => a.DoUploadActionAsync(It.IsAny<PutObjectRequest>()), Times.Exactly(2));
        }

        [Test]
        public async Task MultipleFileFolderTest_AllUploadsFail()
        {
            const string folderPath = "Garbage";
            SetUpRemotePath(folderPath, folderPath);

            const string filePath1 = "Garbage/Trash.txt";
            SetUpFile(filePath1, filePath1, HttpStatusCode.BadRequest);

            const string filePath2 = "Garbage/Rubbish.txt";
            SetUpFile(filePath2, filePath2, HttpStatusCode.BadRequest);

            SetUpGetFilePathsForFolder(folderPath, filePath1, filePath2);

            Should.Throw<AWSFileUploadException>(async () => await Uploader.UploadFolderAsync(folderPath));

            MockActionTaker.Verify(a => a.DoUploadActionAsync(It.IsAny<PutObjectRequest>()), Times.Exactly(2));
        }

        [Test]
        public async Task MultipleFileFolderTest_OneFailOnePass()
        {
            const string folderPath = "Garbage";
            SetUpRemotePath(folderPath, folderPath);

            const string filePath1 = "Garbage/Trash.txt";
            SetUpFile(filePath1, filePath1, HttpStatusCode.OK);

            const string filePath2 = "Garbage/Rubbish.txt";
            SetUpFile(filePath2, filePath2, HttpStatusCode.BadRequest);

            SetUpGetFilePathsForFolder(folderPath, filePath1, filePath2);

            Should.Throw<AWSFileUploadException>(async () => await Uploader.UploadFolderAsync(folderPath));

            MockActionTaker.Verify(a => a.DoUploadActionAsync(It.IsAny<PutObjectRequest>()), Times.Exactly(2));
        }
    }
}