using FolderSynchronizer.Data;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileListerImpTests
{
    public class GetFileDataAsyncTests : AWSFileListerImpTestBase
    {
        [Test]
        public async Task EmptyListTest()
        {
            SetUpMockActionTakerToReturnResponseWithS3Objects();

            var result = await FileLister.GetFileDataAsync();

            result.ShouldBeEmpty();
        }

        [Test]
        public async Task SingleItemListTest()
        {
            const string path = "Test";
            var lastModifiedDate = new DateTime(2021, 11, 27).ToUniversalTime();
            var testObject = CreateS3Object(path, lastModifiedDate);
            SetUpMockActionTakerToReturnResponseWithS3Objects(testObject);

            var result = await FileLister.GetFileDataAsync();

            result.Count().ShouldBe(1);

            var data = result.Single();

            CheckFileData(data, path, lastModifiedDate);
        }

        private void CheckFileData(FileData data, string expectedPath, DateTime expectedLastModifiedDate)
        {
            data.Path.ShouldBe(expectedPath);
            data.LastModifiedDate.ShouldBe(expectedLastModifiedDate);
        }

        [Test]
        public async Task MultipleItemListTest()
        {
            const string path1 = "Test";
            var lastModifiedDate1 = new DateTime(2021, 11, 27).ToUniversalTime();
            var testObject1 = CreateS3Object(path1, lastModifiedDate1);

            const string path2 = "Test2";
            var lastModifiedDate2 = new DateTime(2021, 11, 28).ToUniversalTime();
            var testObject2 = CreateS3Object(path2, lastModifiedDate2);

            SetUpMockActionTakerToReturnResponseWithS3Objects(testObject1, testObject2);

            var result = (await FileLister.GetFileDataAsync()).ToList();

            result.Count().ShouldBe(2);

            CheckFileData(result[0], path1, lastModifiedDate1);
            CheckFileData(result[1], path2, lastModifiedDate2);
        }
    }
}