using FolderSynchronizer.Data;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.FileDataListPersisterImpTests
{
    public class LoadAsyncTests : FileDataListPersisterImpTestBase
    {
        public override void SetUp()
        {
            base.SetUp();

            WriteToFile();
        }

        private void WriteToFile()
        {
            File.WriteAllTextAsync(FilePath, TestFileContents);
        }

        private string TestFileContents => "Test";

        [Test]
        public async Task NoItemsReturned()
        {
            var expectedData = Array.Empty<FileData>();
            SetUpMockSerializer(expectedData);

            var data = await FileDataListPersister.LoadAsync();
            data.ShouldBeEquivalentTo(expectedData);
        }

        private void SetUpMockSerializer(FileData[] data)
        {
            MockSerializer.Setup(s => s.Deserialize<IEnumerable<FileData>>(TestFileContents)).Returns(data);
        }

        [Test]
        public async Task OneItemReturned()
        {
            var expectedData = new FileData[] { new FileData { } };
            SetUpMockSerializer(expectedData);

            var data = await FileDataListPersister.LoadAsync();
            data.ShouldBeEquivalentTo(expectedData);
        }

        [Test]
        public async Task ThreeItemsReturned()
        {
            var expectedData = new FileData[] { new FileData { }, new FileData { }, new FileData { } };
            SetUpMockSerializer(expectedData);

            var data = await FileDataListPersister.LoadAsync();
            data.ShouldBeEquivalentTo(expectedData);
        }
    }
}
