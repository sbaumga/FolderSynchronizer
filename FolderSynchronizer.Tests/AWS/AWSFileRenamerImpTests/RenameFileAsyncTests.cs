﻿using FolderSynchronizer.AWS.Exceptions;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileRenamerImpTests
{
    public class RenameFileAsyncTests : AWSFileRenamerImpTestBase
    {
        [Test]
        public async Task RenameFile_Success()
        {
            SetUpSuccessfulFileUpload();
            SetUpSuccessfulDeletion();

            await Renamer.RenameFileAsync(OldPath, NewPath);

            VerifyUploadFileAttempted();
            VerifyDeletionAttempted();
        }

        private void SetUpSuccessfulFileUpload()
        {
            SetUpPathIsFile(true);
            MockUploader.Setup(m => m.UploadFileAsync(NewPath)).Returns(Task.CompletedTask);
        }

        private string NewPath = "Trash";

        private void SetUpPathIsFile(bool isFile)
        {
            MockPathManager.Setup(m => m.IsPathFile(NewPath)).Returns(isFile);
        }

        private void SetUpSuccessfulDeletion()
        {
            MockDeleter.Setup(m => m.DeleteRemoteFileFromLocalFileAsync(OldPath)).Returns(Task.CompletedTask);
        }

        private string OldPath = "Garbage";

        private void VerifyUploadFileAttempted()
        {
            MockUploader.Verify(m => m.UploadFileAsync(NewPath), Times.Once);
        }

        private void VerifyDeletionAttempted()
        {
            MockDeleter.Verify(m => m.DeleteRemoteFileFromLocalFileAsync(OldPath), Times.Once);
        }

        [Test]
        public void RenameFile_DeletionFailure()
        {
            SetUpSuccessfulFileUpload();

            SetUpDeletionFailure();

            Should.Throw<NotImplementedException>(async () => await Renamer.RenameFileAsync(OldPath, NewPath));

            VerifyUploadFileAttempted();
            VerifyDeletionAttempted();
        }

        private void SetUpDeletionFailure()
        {
            MockDeleter.Setup(m => m.DeleteRemoteFileFromLocalFileAsync(OldPath)).Throws<NotImplementedException>();
        }

        [Test]
        public void RenameFile_UploadFailure()
        {
            SetUpFailedFileUpload();

            Should.Throw<Exception>(async () => await Renamer.RenameFileAsync(OldPath, NewPath));

            VerifyUploadFileAttempted();
            VerifyDeletionNotAttempted();
        }

        private void SetUpFailedFileUpload()
        {
            SetUpPathIsFile(true);
            MockUploader.Setup(m => m.UploadFileAsync(NewPath)).Throws<AWSFileUploadException>();
        }

        private void VerifyDeletionNotAttempted()
        {
            MockDeleter.Verify(m => m.DeleteRemoteFileFromLocalFileAsync(OldPath), Times.Never);
        }

        [Test]
        public async Task RenameFolder_Success()
        {
            SetUpSuccessfulFolderUpload();
            SetUpSuccessfulDeletion();

            await Renamer.RenameFileAsync(OldPath, NewPath);

            VerifyUploadFolderAttempted();
            VerifyDeletionAttempted();
        }

        private void SetUpSuccessfulFolderUpload()
        {
            SetUpPathIsFile(false);
            MockUploader.Setup(m => m.UploadFolderAsync(NewPath)).Returns(Task.CompletedTask);
        }

        private void VerifyUploadFolderAttempted()
        {
            MockUploader.Verify(m => m.UploadFolderAsync(NewPath), Times.Once);
        }

        [Test]
        public void RenameFolder_DeletionFailure()
        {
            SetUpSuccessfulFolderUpload();

            SetUpDeletionFailure();

            Should.Throw<NotImplementedException>(async () => await Renamer.RenameFileAsync(OldPath, NewPath));

            VerifyUploadFolderAttempted();
            VerifyDeletionAttempted();
        }

        [Test]
        public void RenameFolder_UploadFailure()
        {
            SetUpFailedFolderUpload();

            Should.Throw<Exception>(async () => await Renamer.RenameFileAsync(OldPath, NewPath));

            VerifyUploadFolderAttempted();
            VerifyDeletionNotAttempted();
        }

        private void SetUpFailedFolderUpload()
        {
            SetUpPathIsFile(false);
            MockUploader.Setup(m => m.UploadFolderAsync(NewPath)).Throws<AWSFileUploadException>();
        }
    }
}
