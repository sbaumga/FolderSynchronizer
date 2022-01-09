using NUnit.Framework;
using System.IO;

namespace FolderSynchronizer.Tests
{
    [TestFixture]
    public abstract class LocalFolderTestBase
    {
        [SetUp]
        public virtual void SetUp()
        {
            CreateBaseTestFolder();
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

        protected string TestFolderName = "LocalFileListerImpTest Files";

        private string GetCurrentFolderPath()
        {
            return Directory.GetCurrentDirectory();
        }

        private void DeleteTestFolder()
        {
            var testFolderPath = GetTestFolderPath();
            Directory.Delete(testFolderPath, true);
        }

        [TearDown]
        public virtual void TearDown()
        {
            DeleteTestFolder();
        }
    }
}