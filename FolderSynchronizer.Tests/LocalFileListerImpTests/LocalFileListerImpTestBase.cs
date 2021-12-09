using FolderSynchronizer.Implementations;
using NUnit.Framework;
using System;
using System.IO;

namespace FolderSynchronizer.Tests.LocalFileListerImpTests
{
    [TestFixture]
    public abstract class LocalFileListerImpTestBase
    {
        protected LocalFileListerImp FileLister { get; set; }

        [SetUp]
        public void SetUp()
        {
            CreateBaseTestFolder();

            FileLister = new LocalFileListerImp();
        }

        private void CreateBaseTestFolder()
        {
            var testFolderPath = GetTestFolderPath();

            if (Directory.Exists(testFolderPath))
            {
                DeleteTestFolder();
            }

            Directory.CreateDirectory(testFolderPath);
        }

        protected string GetTestFolderPath()
        {
            var currentFolder = GetCurrentFolderPath();
            return Path.Combine(currentFolder, TestFolderName);
        }

        private string GetCurrentFolderPath()
        {
            return Directory.GetCurrentDirectory();
        }

        protected string TestFolderName = "LocalFileListerImpTest Files";

        [TearDown]
        public void TearDown()
        {
            DeleteTestFolder();
        }

        private void DeleteTestFolder()
        {
            var testFolderPath = GetTestFolderPath();
            Directory.Delete(testFolderPath, true);
        }
    }
}