using Amazon.S3.Model;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileDeleterImpTests
{
    public abstract class FileDeletionTestBase : AWSFileDeleterImpTestBase
    {
        protected abstract string DeletionPath { get; }

        [Test]
        public virtual void DeleteFile_Failure()
        {
            SetUpPathIsFile(DeletionPath, true);
            SetUpDoDeleteAction(DeletionPath, HttpStatusCode.BadRequest);

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyDeletionActionTaken(DeletionPath, Times.Once);

            VerifyFailedDeletionLogs(DeletionPath);
        }

        protected abstract void DeletionFunction(string path);

        private void VerifyDeletionActionTaken(string fileKey, Func<Times> times)
        {
            MockActionTaker.Verify(a => a.DoDeleteAction(It.Is<DeleteObjectRequest>(r => r.Key == fileKey)), times);
        }

        private void VerifyFailedDeletionLogs(string path)
        {
            VerifyDeletionStartLogged(path);
            VerifyDeletionEndNotLogged(path);
        }

        private void VerifyDeletionStartLogged(string path)
        {
            MockLogger.Verify(l => l.LogInformation(It.Is<string>(s => s == $"Deleting file {path}")), Times.Once);
        }

        private string EndDeleteLogMessage(string path)
        {
            return $"File {path} successfully deleted";
        }

        private void VerifyDeletionEndNotLogged(string path)
        {
            MockLogger.Verify(l => l.LogInformation(It.Is<string>(s => s == EndDeleteLogMessage(path))), Times.Never);
        }

        [Test]
        public virtual void DeleteFile_Success()
        {
            SetUpPathIsFile(DeletionPath, true);
            SetUpDoDeleteAction(DeletionPath, HttpStatusCode.OK);

            DeletionFunction(DeletionPath);

            VerifyDeletionActionTaken(DeletionPath, Times.Once);
            VerifySuccessfulDeletionLogs(DeletionPath);
        }

        private void VerifySuccessfulDeletionLogs(string path)
        {
            VerifyDeletionStartLogged(path);
            VerifyDeletionEndLogged(path);
        }

        private void VerifyDeletionEndLogged(string path)
        {
            MockLogger.Verify(l => l.LogInformation(It.Is<string>(s => s == EndDeleteLogMessage(path))), Times.Once);
        }

        [Test]
        public void DeleteEmptyFolder()
        {
            SetUpPathIsFile(DeletionPath, false);
            SetUpFileListerToReturnFiles(FolderPath, Enumerable.Empty<string>());

            DeletionFunction(DeletionPath);

            VerifyDeletionActionTaken(DeletionPath, Times.Never);
            MockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Never);
        }

        protected abstract string FolderPath { get; }

        private void SetUpFileListerToReturnFiles(string remotePath, IEnumerable<string> filesInFolderPaths)
        {
            MockFileLister.Setup(l => l.ListFilteredFilesAsync(remotePath)).Returns(Task.FromResult(filesInFolderPaths));
        }

        [Test]
        public virtual void DeleteSingleFileFolder_Success()
        {
            SingleFileFolderSetUp(HttpStatusCode.OK);

            DeletionFunction(DeletionPath);

            VerifyDeletionActionTaken(SingleFilePath, Times.Once);
            VerifySuccessfulDeletionLogs(SingleFilePath);
        }

        protected string SingleFilePath => "Rubbish";

        private void SingleFileFolderSetUp(HttpStatusCode responseCode)
        {
            SetUpPathIsFile(DeletionPath, false);
            SetUpDoDeleteAction(SingleFilePath, responseCode);

            SetUpFileListerToReturnFiles(FolderPath, new List<string> { SingleFilePath });
        }

        [Test]
        public virtual void DeleteSingleFileFolder_Failure()
        {
            SingleFileFolderSetUp(HttpStatusCode.BadRequest);

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyDeletionActionTaken(SingleFilePath, Times.Once);
            VerifyFailedDeletionLogs(SingleFilePath);
        }

        [Test]
        public virtual void MultipleFileFolder_AllUploadsSucceed()
        {
            MultipleFileFolderSetUp(MultipleFileFolderFilePaths.Select(p => Tuple.Create(p, HttpStatusCode.OK)));

            Should.NotThrow(() => DeletionFunction(DeletionPath));

            VerifyMultipleFileFolderDeletesAttempted();
            foreach(var path in MultipleFileFolderFilePaths)
            {
                VerifySuccessfulDeletionLogs(path);
            }
        }

        protected string[] MultipleFileFolderFilePaths = new[] { "Rubbish", "Crud", "Waste" };

        private void MultipleFileFolderSetUp(IEnumerable<Tuple<string, HttpStatusCode>> deleteResponsesForPaths)
        {
            SetUpPathIsFile(DeletionPath, false);

            foreach (var pair in deleteResponsesForPaths)
            {
                SetUpPathIsFile(pair.Item1, true);
                SetUpDoDeleteAction(pair.Item1, pair.Item2);
            }
            AdditionalMultipleFileFolderFilePathsSetUp();

            SetUpFileListerToReturnFiles(FolderPath, MultipleFileFolderFilePaths);
        }

        protected virtual void AdditionalMultipleFileFolderFilePathsSetUp()
        { }

        private void VerifyMultipleFileFolderDeletesAttempted()
        {
            foreach (var filePath in MultipleFileFolderFilePaths)
            {
                VerifyDeletionActionTaken(filePath, Times.Once);
            }
        }

        [Test]
        public virtual void MultipleFileFolder_AllUploadsFail()
        {
            MultipleFileFolderSetUp(MultipleFileFolderFilePaths.Select(p => Tuple.Create(p, HttpStatusCode.BadRequest)));

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyMultipleFileFolderDeletesAttempted();
            foreach (var path in MultipleFileFolderFilePaths)
            {
                VerifyFailedDeletionLogs(path);
            }
        }

        [Test]
        public virtual void MultipleFileFolder_SomeUploadsFail()
        {
            MultipleFileFolderSetUp(MixedSuccessTest_FailurePaths.Select(p => Tuple.Create(p, HttpStatusCode.BadRequest)));
            MultipleFileFolderSetUp(MixedSuccessTest_SuccessPaths.Select(p => Tuple.Create(p, HttpStatusCode.OK)));

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyMultipleFileFolderDeletesAttempted();

            foreach (var path in MixedSuccessTest_FailurePaths)
            {
                VerifyFailedDeletionLogs(path);
            }

            foreach (var path in MixedSuccessTest_SuccessPaths)
            {
                VerifySuccessfulDeletionLogs(path);
            }
        }

        protected IEnumerable<string> MixedSuccessTest_FailurePaths => MultipleFileFolderFilePaths.Where((p, i) => i % 2 != 0);

        protected IEnumerable<string> MixedSuccessTest_SuccessPaths => MultipleFileFolderFilePaths.Where((p, i) => i % 2 == 0);
    }
}