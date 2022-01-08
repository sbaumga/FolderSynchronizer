using FolderSynchronizer.Implementations;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderSynchronizer.Tests.LocalFileListerImpTests
{
    public abstract class LocalFileListerImpTestBase<T> : LocalFolderTestBase
    {
        protected LocalFileListerImp FileLister { get; set; }

        public override void SetUp()
        {
            base.SetUp();

            FileLister = new LocalFileListerImp();
        }

        [Test]
        public void FolderDoesNotExistTest()
        {
            Should.Throw<DirectoryNotFoundException>(() => PerformTestableFunctionOnPath("Garbage"));
        }

        protected abstract IEnumerable<T> PerformTestableFunctionOnPath(string path);

        [Test]
        public void EmptyFolderTest()
        {
            var result = RunTestableFunctionForFolderOnTestFolder();

            result.ShouldBeEmpty();
        }

        private IEnumerable<T> RunTestableFunctionForFolderOnTestFolder()
        {
            var folderPath = GetTestFolderPath();
            var result = PerformTestableFunctionOnPath(folderPath);

            return result;
        }

        protected void CreateFileInTestFolder(string fileName)
        {
            var filePath = GetFullExpectedPathForFile(fileName);
            using var _ = File.Create(filePath);
        }

        protected string GetFullExpectedPathForFile(string fileName)
        {
            var folderPath = GetTestFolderPath();
            var filePath = Path.Combine(folderPath, fileName);
            return filePath;
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

            var result = RunTestableFunctionForFolderOnTestFolder().ToList();

            result.Count.ShouldBe(fileNames.Length);

            foreach (var file in expectedFilePaths)
            {
                VerifyFilePathExistsInTestableFunctionResult(file, result);
            }
        }

        protected abstract void VerifyFilePathExistsInTestableFunctionResult(string filePath, IEnumerable<T> testableFunctionResult);

        [Test]
        public void ThreeFileTest()
        {
            DoFileTest("TestFile1.txt", "TestFile2.mp3", "TestFile3.jpg");
        }
    }
}