using Amazon.S3.Model;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileListerImpTests
{
    public class ListFilteredFilesAsyncTests : AWSFileListerImpTestBase
    {
        [Test]
        public async Task EmptyListTest()
        {
            SetUpMockActionTakerToReturnResponseWithS3Objects();

            const string filter = "Test";

            var result = await FileLister.ListFilteredFilesAsync(filter);

            result.ShouldBeEmpty();
        }

        [Test]
        public async Task SingleItemListTest()
        {
            var testObject = CreateS3Object("Test");
            SetUpMockActionTakerToReturnResponseWithS3Objects(testObject);

            const string filter = "Test";

            var result = await FileLister.ListFilteredFilesAsync(filter);

            result.ShouldHaveSingleItem(testObject.Key);
        }

        [Test]
        public async Task MultipleItemListTest()
        {
            var testObject1 = CreateS3Object("Test");
            var testObject2 = CreateS3Object("Test2");
            SetUpMockActionTakerToReturnResponseWithS3Objects(testObject1, testObject2);

            const string filter = "Test";

            var result = await FileLister.ListFilteredFilesAsync(filter);

            result.Count().ShouldBe(2);

            result.ShouldContain(testObject1.Key);
            result.ShouldContain(testObject2.Key);
        }

        [Test]
        public async Task FilterOutItemsTest()
        {
            const string filter = "Test";
            var s3Objects = CreateS3ObjectsForFilterOutTest(filter);
            SetUpMockActionTakerToReturnResponseWithS3Objects(s3Objects);

            var result = await FileLister.ListFilteredFilesAsync(filter);

            var matchingObjects = s3Objects.Where(o => o.Key.StartsWith(filter));

            result.Count().ShouldBe(matchingObjects.Count());

            result.ToList().ShouldBeEquivalentTo(matchingObjects.Select(o => o.Key).ToList());
        }

        private S3Object[] CreateS3ObjectsForFilterOutTest(string filter)
        {
            var startsWithFilter1 = CreateS3Object(filter);
            var doesNotContainFilter = CreateS3Object(filter != "Garbage" ? "Garbage" : "Trash");
            var startsWithFilter2 = CreateS3Object($"{filter}2");
            var endsWithFilter = CreateS3Object($"Not{filter}");

            return new[] { startsWithFilter1, doesNotContainFilter, startsWithFilter2, endsWithFilter };
        }
    }
}