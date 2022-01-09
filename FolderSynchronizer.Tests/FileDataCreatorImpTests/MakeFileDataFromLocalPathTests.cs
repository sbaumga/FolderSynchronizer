using FolderSynchronizer.Implementations;
using NUnit.Framework;
using Shouldly;
using System;
using System.IO;

namespace FolderSynchronizer.Tests.FileDataCreatorImpTests
{
    public class MakeFileDataFromLocalPathTests : LocalFolderTestBase
    {
        private FileDataCreatorImp DataCreator { get; set; }

        public override void SetUp()
        {
            base.SetUp();

            DataCreator = new FileDataCreatorImp();
        }

        [Test]
        public void FileNotFoundTest()
        {
            Should.Throw<FileNotFoundException>(() => DataCreator.MakeFileDataFromLocalPath("NotAPath"));
        }

        [Test]
        public void SuccessTest()
        {
            var path = TestFilePath;
            CreateFile(path);
            var creationDate = DateTime.UtcNow;

            var data = DataCreator.MakeFileDataFromLocalPath(path);

            data.Path.ShouldBe(path);
            CompareLastModifiedDate(creationDate, data.LastModifiedDate);

            File.Delete(path);
        }

        private void CreateFile(string path)
        {
            using var fileWriter = new FileStream(path, FileMode.Create);
            fileWriter.Flush();
        }

        private string TestFilePath => Path.Combine(GetTestFolderPath(), "TestFile.txt");

        private void CompareLastModifiedDate(DateTime timeAfterFileCreation, DateTime lastModifiedDate)
        {
            // DateTime.Now does not return milliseconds
            var modifiedDateWithoutMilliseconds = TruncateMilliseconds(lastModifiedDate);
            modifiedDateWithoutMilliseconds.ShouldBeGreaterThan(timeAfterFileCreation.AddSeconds(-1));
            modifiedDateWithoutMilliseconds.ShouldBeLessThanOrEqualTo(timeAfterFileCreation);
        }

        private DateTime TruncateMilliseconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond), dateTime.Kind);
        }
    }
}