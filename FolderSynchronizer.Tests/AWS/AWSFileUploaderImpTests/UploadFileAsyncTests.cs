using Amazon.S3.Model;
using FolderSynchronizer.AWS.Exceptions;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileUploaderImpTests
{
    public class UploadFileAsyncTests : AWSFileUploaderImpTestBase
    {
        [Test]
        public async Task UploadingFolderTest()
        {
            SetUpIsPathFile(false);

            Should.NotThrow(async () => await Uploader.UploadFileAsync("Garbage"));
        }

        [Test]
        public async Task UploadReturnsErrorCodeTest()
        {
            SetUpIsPathFile(true);

            const string localPath = "Garbage\\Trash.txt";

            SetUpBasicUploadTest(localPath, System.Net.HttpStatusCode.BadRequest);

            Should.Throw<AWSFileUploadException>(async () => await Uploader.UploadFileAsync(localPath));
        }

        private void SetUpBasicUploadTest(string localPath, System.Net.HttpStatusCode responseCode)
        {
            const string remotePath = "Garbage/Trash.txt";

            MockPathManager.Setup(m => m.GetRemotePath(localPath)).Returns(remotePath);

            var response = new PutObjectResponse
            {
                HttpStatusCode = responseCode
            };

            MockActionTaker.Setup(t => t.DoUploadActionAsync(It.IsAny<PutObjectRequest>())).Returns(Task.FromResult(response)).Callback<PutObjectRequest>(request =>
            {
                request.FilePath.ShouldBe(localPath);
                request.Key.ShouldBe(remotePath);
                request.BucketName.ShouldBe(BucketName);
            });
        }

        [Test]
        public async Task UploadSucceedsTest()
        {
            SetUpIsPathFile(true);

            const string localPath = "Garbage\\Trash.txt";

            SetUpBasicUploadTest(localPath, System.Net.HttpStatusCode.OK);

            Should.NotThrow(async () => await Uploader.UploadFileAsync(localPath));
        }
    }
}