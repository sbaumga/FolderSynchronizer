using Amazon.S3.Model;
using FolderSynchronizer.AWS.Exceptions;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileUploaderImpTests
{
    public class UploadFileAsyncTests : AWSFileUploaderImpTestBase
    {
        [Test]
        public async Task NullPathTest()
        {
            Should.Throw<ArgumentNullException>(async () => await Uploader.UploadFileAsync(null));
        }

        [Test]
        public async Task EmptyPathTest()
        {
            Should.Throw<ArgumentNullException>(async () => await Uploader.UploadFileAsync(string.Empty));
        }

        [Test]
        public async Task UploadingFolderTest()
        {
            const string path = "Garbage";
            SetUpIsPathFile(false, path);

            Should.NotThrow(async () => await Uploader.UploadFileAsync(path));
        }

        [Test]
        public async Task UploadReturnsErrorCodeTest()
        {
            const string localPath = "Garbage\\Trash.txt";
            SetUpIsPathFile(true, localPath);

            SetUpBasicFileUploadTest(localPath, System.Net.HttpStatusCode.BadRequest);

            Should.Throw<AWSFileUploadException>(async () => await Uploader.UploadFileAsync(localPath));
        }

        private void SetUpBasicFileUploadTest(string localPath, System.Net.HttpStatusCode responseCode)
        {
            const string remotePath = "Garbage/Trash.txt";

            SetUpRemotePath(localPath, remotePath);

            SetUpUploadAction(responseCode, localPath, remotePath);
        }

        [Test]
        public async Task UploadSucceedsTest()
        {
            const string localPath = "Garbage\\Trash.txt";
            SetUpIsPathFile(true, localPath);

            SetUpBasicFileUploadTest(localPath, System.Net.HttpStatusCode.OK);

            Should.NotThrow(async () => await Uploader.UploadFileAsync(localPath));
        }
    }
}