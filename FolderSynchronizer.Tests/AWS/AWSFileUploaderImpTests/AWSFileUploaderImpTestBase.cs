using Amazon.S3.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Net;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileUploaderImpTests
{
    [TestFixture]
    public abstract class AWSFileUploaderImpTestBase
    {
        protected Mock<ILogger<AWSFileUploaderImp>> MockLogger { get; set; }

        protected Mock<IAWSPathManager> MockPathManager { get; set; }
        protected Mock<IAWSActionTaker> MockActionTaker { get; set; }
        protected Mock<ILocalFileLister> MockLocalFileLister { get; set; }

        protected string BucketName { get; set; }

        protected AWSFileUploaderImp Uploader { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockLogger = new Mock<ILogger<AWSFileUploaderImp>>();
            MockLogger.Setup(l => l.LogInformation(It.IsAny<string>()));

            MockPathManager = new Mock<IAWSPathManager>(MockBehavior.Strict);

            MockActionTaker = new Mock<IAWSActionTaker>(MockBehavior.Strict);

            MockLocalFileLister = new Mock<ILocalFileLister>(MockBehavior.Strict);

            BucketName = "TestBucket";
            var configData = new ConfigData { BucketName = BucketName };

            Uploader = new AWSFileUploaderImp(MockLogger.Object, MockPathManager.Object, MockActionTaker.Object, MockLocalFileLister.Object, configData);
        }

        protected void SetUpIsPathFile(bool returnValue, string path)
        {
            MockPathManager.Setup(p => p.IsPathFile(path)).Returns(returnValue);
        }

        protected void SetUpRemotePath(string localPath, string remotePath)
        {
            MockPathManager.Setup(m => m.GetRemotePath(localPath)).Returns(remotePath);
        }

        protected void SetUpGetFilePathsForFolder(string folderPath, params string[] folderFiles)
        {
            MockLocalFileLister.Setup(l => l.GetFilePathsForFolder(folderPath)).Returns(folderFiles);
        }

        protected void SetUpUploadAction(HttpStatusCode statusCode, string expectedLocalPath, string expectedRemotePath)
        {
            var response = new PutObjectResponse
            {
                HttpStatusCode = statusCode,
            };

            MockActionTaker.Setup(t => t.DoUploadActionAsync(It.Is<PutObjectRequest>(r => r.FilePath == expectedLocalPath))).Returns(Task.FromResult(response)).Callback<PutObjectRequest>(request =>
            {
                request.FilePath.ShouldBe(expectedLocalPath);
                request.Key.ShouldBe(expectedRemotePath);
                request.BucketName.ShouldBe(BucketName);
            });
        }
    }
}