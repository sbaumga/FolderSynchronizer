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
        [Test]
        public async Task NoFileTest()
        {
            var data = await FileDataListPersister.LoadAsync();
            data.ShouldBeEmpty();
        }

        [Test]
        public async Task NoItemsReturned()
        {
            WriteToFile();

            var expectedData = Array.Empty<FileData>();
            SetUpMockSerializer(expectedData);

            var data = await FileDataListPersister.LoadAsync();
            data.ShouldBeEmpty();
        }

        private void WriteToFile()
        {
            File.WriteAllTextAsync(FilePath, TestFileContents);
        }

        private string TestFileContents => "Test";

        private void SetUpMockSerializer(FileData[] data)
        {
            MockSerializer.Setup(s => s.Deserialize<IEnumerable<FileData>>(TestFileContents)).Returns(data);
        }

        [Test]
        public async Task OneItemReturned()
        {
            WriteToFile();

            var expectedData = new FileData[] { new FileData { } };
            SetUpMockSerializer(expectedData);

            var data = await FileDataListPersister.LoadAsync();
            data.ShouldBeEquivalentTo(expectedData);
        }

        [Test]
        public async Task ThreeItemsReturned()
        {
            WriteToFile();

            var expectedData = new FileData[] { new FileData { }, new FileData { }, new FileData { } };
            SetUpMockSerializer(expectedData);

            var data = await FileDataListPersister.LoadAsync();
            data.ShouldBeEquivalentTo(expectedData);
        }
    }
}
