using Amazon.S3;
using Amazon.S3.Model;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileListerImpTests
{
    [TestFixture]
    public class ListFilesAsyncTests
    {
        private Mock<IAWSActionTaker> MockActionTaker { get; set; }

        private ConfigData ConfigData { get; } = new ConfigData { BucketName = "TestBucket" };

        private AWSFileListerImp FileLister { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockActionTaker = new Mock<IAWSActionTaker>(MockBehavior.Strict);

            FileLister = new AWSFileListerImp(MockActionTaker.Object, ConfigData);
        }

        [Test]
        public async Task EmptyListTest()
        {
            SetUpMockActionTakerToReturnResponseWithS3Objects();

            var result = await FileLister.ListFilesAsync();

            result.ShouldBeEmpty();
        }

        private void SetUpMockActionTakerToReturnResponseWithS3Objects(params S3Object[] s3Objects)
        {
            var fakeResponse = new ListObjectsV2Response
            {
                S3Objects = new List<S3Object>(s3Objects)
            };

            MockActionTaker.Setup(s => s.DoS3Action(It.IsAny<Func<AmazonS3Client, Task<ListObjectsV2Response>>>())).Returns(Task.FromResult(fakeResponse));
        }

        [Test]
        public async Task SingleItemListTest()
        {
            var testObject = CreateS3Object("Test");
            SetUpMockActionTakerToReturnResponseWithS3Objects(testObject);

            var result = await FileLister.ListFilesAsync();

            result.ShouldHaveSingleItem(testObject.Key);
        }

        private S3Object CreateS3Object(string key)
        {
            var s3Object = new S3Object
            {
                Key = key
            };
            return s3Object;
        }

        [Test]
        public async Task MultipleItemListTest()
        {
            var testObject1 = CreateS3Object("Test");
            var testObject2 = CreateS3Object("Test2");
            SetUpMockActionTakerToReturnResponseWithS3Objects(testObject1, testObject2);

            var result = (await FileLister.ListFilesAsync()).ToList();

            result.Count().ShouldBe(2);

            result.ShouldContain(testObject1.Key);
            result.ShouldContain(testObject2.Key);
        }
    }
}