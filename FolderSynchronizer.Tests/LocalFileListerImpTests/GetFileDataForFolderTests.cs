using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderSynchronizer.Tests.LocalFileListerImpTests
{
    // TODO: rework tests into base class
    public class GetFileDataForFolderTests : LocalFileListerImpTestBase
    {
        [Test]
        public void FolderDoesNotExistTest()
        {
            Should.Throw<DirectoryNotFoundException>(() => FileLister.GetFileDataForFolder("Garbage"));
        }

        [Test]
        public void EmptyFolderTest()
        {
            var result = RunGetFileDataForFolderOnTestFolder();

            result.ShouldBeEmpty();
        }

        private IEnumerable<FileData> RunGetFileDataForFolderOnTestFolder()
        {
            var folderPath = GetTestFolderPath();
            var result = FileLister.GetFileDataForFolder(folderPath);

            return result;
        }

        [Test]
        public void SingleFileTest()
        {
            DoFileTest("TestFile.txt");
        }

        private void DoFileTest(params string[] fileNames)
        {
            foreach (var file in fileNames)
            {
                CreateFileInTestFolder(file);
            }

            var expectedFilePaths = fileNames.Select(n => GetFullExpectedPathForFile(n));

            var result = RunGetFileDataForFolderOnTestFolder().ToList();

            result.Count().ShouldBe(fileNames.Length);

            foreach (var file in expectedFilePaths)
            {
                result.ShouldContain(d => d.Path == file && d.LastModifiedDate > System.DateTime.Now.AddMinutes(2));
            }
        }

        [Test]
        public void ThreeFileTest()
        {
            DoFileTest("TestFile1.txt", "TestFile2.mp3", "TestFile3.jpg");
        }
    }
}