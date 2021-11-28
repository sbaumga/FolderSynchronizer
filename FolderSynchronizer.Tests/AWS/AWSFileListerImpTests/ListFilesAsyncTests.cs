using NUnit.Framework;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileListerImpTests
{
    public class ListFilesAsyncTests : AWSFileListerImpTestBase
    {
        [Test]
        public async Task EmptyListTest()
        {
            SetUpMockActionTakerToReturnResponseWithS3Objects();

            var result = await FileLister.ListFilesAsync();

            result.ShouldBeEmpty();
        }

        [Test]
        public async Task SingleItemListTest()
        {
            var testObject = CreateS3Object("Test");
            SetUpMockActionTakerToReturnResponseWithS3Objects(testObject);

            var result = await FileLister.ListFilesAsync();

            result.ShouldHaveSingleItem(testObject.Key);
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