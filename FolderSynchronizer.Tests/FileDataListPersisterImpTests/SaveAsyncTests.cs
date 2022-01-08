using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.FileDataListPersisterImpTests
{
    public class SaveAsyncTests : FileDataListPersisterImpTestBase
    {
        public override void SetUp()
        {
            base.SetUp();

            SetUpMockSerializer();
        }

        private void SetUpMockSerializer()
        {
            MockSerializer.Setup(s => s.Serialize(It.IsAny<IEnumerable<FileData>>())).Returns(TestSerializedData);
        }

        protected string TestSerializedData => "Test Data";

        [Test]
        public async Task EmptyListTest()
        {
            var input = Enumerable.Empty<FileData>();

            await FileDataListPersister.SaveAsync(input);

            await VerifyFileContentsCorrect();
        }

        private async Task VerifyFileContentsCorrect()
        {
            var fileContents = await File.ReadAllTextAsync(FilePath);
            fileContents.ShouldBe(TestSerializedData);
        }

        [Test]
        public async Task SingleElementListTest()
        {
            var input = new List<FileData> { new FileData() };

            await FileDataListPersister.SaveAsync(input);

            await VerifyFileContentsCorrect();
        }

        [Test]
        public async Task ThreeElementListTest()
        {
            var input = new List<FileData> { new FileData(), new FileData(), new FileData() };

            await FileDataListPersister.SaveAsync(input);

            await VerifyFileContentsCorrect();
        }
    }
}