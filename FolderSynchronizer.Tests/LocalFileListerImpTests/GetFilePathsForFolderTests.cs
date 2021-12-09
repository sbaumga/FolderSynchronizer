﻿using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderSynchronizer.Tests.LocalFileListerImpTests
{
    public class GetFilePathsForFolderTests : LocalFileListerImpTestBase
    {
        [Test]
        public void FolderDoesNotExistTest()
        {
            Should.Throw<DirectoryNotFoundException>(() => FileLister.GetFilePathsForFolder("Garbage"));
        }

        [Test]
        public void EmptyFolderTest()
        {
            var result = RunGetFilePathsForFolderOnTestFolder();

            result.ShouldBeEmpty();
        }

        private IEnumerable<string> RunGetFilePathsForFolderOnTestFolder()
        {
            var folderPath = GetTestFolderPath();
            var result = FileLister.GetFilePathsForFolder(folderPath);

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

            var result = RunGetFilePathsForFolderOnTestFolder().ToList();

            result.Count().ShouldBe(fileNames.Length);

            foreach (var file in expectedFilePaths)
            {
                result.ShouldContain(file);
            }
        }

        [Test]
        public void ThreeFileTest()
        {
            DoFileTest("TestFile1.txt", "TestFile2.mp3", "TestFile3.jpg");
        }
    }
}