using Moq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileDeleterImpTests
{
    public class DeleteRemoteFileFromLocalFileAsyncTests : FileDeletionTestBase
    {
        private string RemotePath = "Trash";
        protected override string DeletionPath => RemotePath;
        protected override string FolderPath => RemotePath;

        public override void SetUp()
        {
            base.SetUp();

            SetUpGetRemotePath(DeletionPath, RemotePath);

            MockSavedFileListRecordDeleter.Setup(d => d.DeleteRecordAsync(DeletionPath)).Returns(Task.CompletedTask);
        }

        protected override void DeletionFunction(string path)
        {
            Deleter.DeleteRemoteFileFromLocalFileAsync(path).Wait();
        }

        protected override void AdditionalMultipleFileFolderFilePathsSetUp()
        {
            foreach(var filePath in MultipleFileFolderFilePaths)
            {
                SetUpGetRemotePath(filePath, filePath);
            }
        }

        public override void DeleteFile_Failure()
        {
            base.DeleteFile_Failure();

            VerifySavedFileListNotModified();
        }

        private void VerifySavedFileListNotModified()
        {
            MockSavedFileListRecordDeleter.Verify(d => d.DeleteRecordAsync(It.IsAny<string>()), Times.Never);
        }

        public override void DeleteFile_Success()
        {
            base.DeleteFile_Success();

            VerifySavedFileListUpdatedWithPath(DeletionPath);
        }

        private void VerifySavedFileListUpdatedWithPath(string path)
        {
            MockSavedFileListRecordDeleter.Verify(d => d.DeleteRecordAsync(path), Times.Once);
        }

        public override void DeleteSingleFileFolder_Success()
        {
            base.DeleteSingleFileFolder_Success();

            VerifySavedFileListUpdatedWithPath(SingleFilePath);
        }

        public override void DeleteSingleFileFolder_Failure()
        {
            base.DeleteSingleFileFolder_Failure();

            VerifySavedFileListNotModified();
        }

        public override void MultipleFileFolder_AllUploadsSucceed()
        {
            base.MultipleFileFolder_AllUploadsSucceed();

            foreach(var filePath in MultipleFileFolderFilePaths)
            {
                VerifySavedFileListUpdatedWithPath(filePath);
            }
        }

        public override void MultipleFileFolder_AllUploadsFail()
        {
            base.MultipleFileFolder_AllUploadsFail();

            VerifySavedFileListNotModified();
        }

        public override void MultipleFileFolder_SomeUploadsFail()
        {
            base.MultipleFileFolder_SomeUploadsFail();

            foreach(var filePath in MixedSuccessTest_SuccessPaths)
            {
                VerifySavedFileListUpdatedWithPath(filePath);
            }

            foreach(var filePath in MixedSuccessTest_FailurePaths)
            {
                VerifySavedFileListNotUpdatedWithPath(filePath);
            }
        }

        private void VerifySavedFileListNotUpdatedWithPath(string filePath)
        {
            MockSavedFileListRecordDeleter.Verify(d => d.DeleteRecordAsync(filePath), Times.Never);
        }
    }
}