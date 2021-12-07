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
        public void DeleteFile_Failure()
        {
            SetUpPathIsFile(DeletionPath, true);
            SetUpDoDeleteAction(DeletionPath, HttpStatusCode.BadRequest);

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyDeletionActionTaken(DeletionPath, Times.Once);
        }

        private void VerifyDeletionActionTaken(string fileKey, Func<Times> times)
        {
            MockActionTaker.Verify(a => a.DoDeleteAction(It.Is<DeleteObjectRequest>(r => r.Key == fileKey)), times);
        }

        protected abstract void DeletionFunction(string path);

        [Test]
        public void DeleteFile_Success()
        {
            SetUpPathIsFile(DeletionPath, true);
            SetUpDoDeleteAction(DeletionPath, HttpStatusCode.OK);

            DeletionFunction(DeletionPath);

            VerifyDeletionActionTaken(DeletionPath, Times.Once);
        }

        [Test]
        public void DeleteEmptyFolder()
        {
            SetUpPathIsFile(DeletionPath, false);
            SetUpFileListerToReturnFiles(FolderPath, Enumerable.Empty<string>());

            DeletionFunction(DeletionPath);

            VerifyDeletionActionTaken(DeletionPath, Times.Never);
        }

        protected abstract string FolderPath { get; }

        private void SetUpFileListerToReturnFiles(string remotePath, IEnumerable<string> filesInFolderPaths)
        {
            MockFileLister.Setup(l => l.ListFilteredFilesAsync(remotePath)).Returns(Task.FromResult(filesInFolderPaths));
        }

        [Test]
        public void DeleteSingleFileFolder_Success()
        {
            SingleFileFolderSetUp(HttpStatusCode.OK);

            DeletionFunction(DeletionPath);

            VerifyDeletionActionTaken(SingleFilePath, Times.Once);
        }

        protected string SingleFilePath => "Rubbish";

        private void SingleFileFolderSetUp(HttpStatusCode responseCode)
        {
            SetUpPathIsFile(DeletionPath, false);
            SetUpDoDeleteAction(SingleFilePath, responseCode);

            SetUpFileListerToReturnFiles(FolderPath, new List<string> { SingleFilePath });
        }

        [Test]
        public void DeleteSingleFileFolder_Failure()
        {
            SingleFileFolderSetUp(HttpStatusCode.BadRequest);

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyDeletionActionTaken(SingleFilePath, Times.Once);
        }

        [Test]
        public void MultipleFileFolder_AllUploadsSucceed()
        {
            MultipleFileFolderSetUp(MultipleFileFolderFilePaths.Select(p => Tuple.Create(p, HttpStatusCode.OK)));

            Should.NotThrow(() => DeletionFunction(DeletionPath));

            VerifyMultipleFileFolderDeletesAttempted();
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
        public void MultipleFileFolder_AllUploadsFail()
        {
            MultipleFileFolderSetUp(MultipleFileFolderFilePaths.Select(p => Tuple.Create(p, HttpStatusCode.BadRequest)));

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyMultipleFileFolderDeletesAttempted();
        }

        [Test]
        public void MultipleFileFolder_SomeUploadsFail()
        {
            MultipleFileFolderSetUp(MultipleFileFolderFilePaths.Select((p, i) => Tuple.Create(p, i % 2 == 0 ? HttpStatusCode.OK : HttpStatusCode.BadRequest)));

            Should.Throw<Exception>(() => DeletionFunction(DeletionPath));

            VerifyMultipleFileFolderDeletesAttempted();
        }
    }
}