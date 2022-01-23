using Amazon.S3.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using System.Net;

namespace FolderSynchronizer.Tests.AWS.AWSFileDeleterImpTests
{
    [TestFixture]
    public abstract class AWSFileDeleterImpTestBase
    {
        protected Mock<IAWSPathManager> MockPathManager { get; set; }
        protected Mock<IAWSFileLister> MockFileLister { get; set; }
        protected Mock<IAWSActionTaker> MockActionTaker { get; set; }
        protected Mock<ISavedFileListRecordDeleter> MockSavedFileListRecordDeleter { get; set; }
        protected Mock<ILogger<AWSFileDeleterImp>> MockLogger { get; set; }

        protected string BucketName => "TestBucket";

        protected AWSFileDeleterImp Deleter { get; set; }

        [SetUp]
        public virtual void SetUp()
        {
            MockPathManager = new Mock<IAWSPathManager>(MockBehavior.Strict);

            MockFileLister = new Mock<IAWSFileLister>(MockBehavior.Strict);

            MockActionTaker = new Mock<IAWSActionTaker>(MockBehavior.Strict);

            MockSavedFileListRecordDeleter = new Mock<ISavedFileListRecordDeleter>(MockBehavior.Strict);

            MockLogger = new Mock<ILogger<AWSFileDeleterImp>>(MockBehavior.Strict);
            MockLogger.Setup(l => l.LogInformation(It.IsAny<string>()));

            var configData = new AWSConfigData
            {
                BucketName = BucketName,
            };

            Deleter = new AWSFileDeleterImp(MockPathManager.Object, MockFileLister.Object, MockActionTaker.Object, configData, MockSavedFileListRecordDeleter.Object, MockLogger.Object);
        }

        protected void SetUpGetRemotePath(string localPath, string remotePath)
        {
            MockPathManager.Setup(p => p.GetRemotePath(localPath)).Returns(remotePath);
        }

        protected void SetUpPathIsFile(string path, bool isFile)
        {
            MockPathManager.Setup(p => p.IsPathFile(path)).Returns(isFile);
        }

        protected void SetUpDoDeleteAction(string fileKey, HttpStatusCode responseCode)
        {
            var response = new DeleteObjectResponse
            {
                HttpStatusCode = responseCode
            };
            MockActionTaker.Setup(a => a.DoDeleteAction(It.Is<DeleteObjectRequest>(r => r.Key == fileKey))).Returns(response);
        }
    }
}