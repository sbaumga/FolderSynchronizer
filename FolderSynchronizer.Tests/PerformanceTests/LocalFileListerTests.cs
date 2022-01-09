using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Implementations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FolderSynchronizer.Tests.PerformanceTests
{
    [Explicit("These performance tests run on real data and do not have defined failure cases.")]
    [TestFixture]
    public class LocalFileListerTests
    {
        private string FolderPath => @"D:\Music";

        private ILocalFileLister FileLister { get; set; }

        [SetUp]
        public void SetUp()
        {
            FileLister = new LocalFileListerImp(new FileDataCreatorImp());
        }

        [Test]
        public void FilePathTest()
        {
            Test(FileLister.GetFilePathsForFolder, "file paths");
        }

        private void Test<T>(Func<string, IEnumerable<T>> testFunction, string returnDataPluralName)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var data = testFunction.Invoke(FolderPath);

            stopwatch.Stop();

            Console.WriteLine($"Obtained {data.Count()} {returnDataPluralName} in {stopwatch.Elapsed.Seconds}.{stopwatch.Elapsed.Milliseconds} seconds.");
        }

        [Test]
        public void FileDataTest()
        {
            Test(FileLister.GetFileDataForFolder, "file data");
        }
    }
}