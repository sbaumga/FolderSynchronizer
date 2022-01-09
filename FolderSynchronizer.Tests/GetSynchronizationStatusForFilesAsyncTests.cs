using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests
{
    [TestFixture]
    public abstract class GetSynchronizationStatusForFilesAsyncTestBase<TSyncChecker>
        where TSyncChecker : IFileSynchronizationChecker
    {
        protected TSyncChecker FileSyncChecker { get; set; }

        [SetUp]
        public void SetUp()
        {
            InitializeFileSyncChecker();
        }

        protected abstract void InitializeFileSyncChecker();

        [Test]
        public async Task NoFilesTest()
        {
            SetUpSourceFileData();
            SetUpDestinationFileData();

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            result.ShouldBeEmpty();
        }

        protected abstract void SetUpSourceFileData(params FileData[] data);

        protected abstract void SetUpDestinationFileData(params FileData[] data);

        protected FileData CreateFileData(string name)
        {
            return new FileData
            {
                Path = name,
                LastModifiedDate = DateTime.Now,
            };
        }

        protected abstract void SetUpGetDestinationPath(string sourcePath, string destinationPath);

        [Test]
        public async Task SourceFileWithNoDestination()
        {
            var sourceData = CreateFileData("TestFile");
            SetUpGetDestinationPath(sourceData.Path, "TestFileDestination");
            SetUpSourceFileData(sourceData);

            SetUpDestinationFileData();

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            result.Count().ShouldBe(1);

            var syncData = result.Single();

            syncData.SourceData.ShouldBe(sourceData);
            syncData.DestinationData.ShouldBeNull();
        }

        [Test]
        public async Task DestinationFileWithNoSource()
        {
            SetUpSourceFileData();

            var remoteData = CreateFileData("TestFile");
            SetUpDestinationFileData(remoteData);

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            result.Count().ShouldBe(1);

            var syncData = result.Single();

            syncData.SourceData.ShouldBeNull();
            syncData.DestinationData.ShouldBe(remoteData);
        }

        [Test]
        public async Task OneSourceAndDestinationFileMatch()
        {
            var fileData = CreateSourceAndDestinationFiles(Tuple.Create("TestFile", "TestFile"));

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            VerifyFileData(fileData, result);
        }

        protected IList<Tuple<FileData, FileData>> CreateSourceAndDestinationFiles(params Tuple<string, string>[] sourceAndDestinationPathPairs)
        {
            var result = new List<Tuple<FileData, FileData>>();

            var sourceDataList = new List<FileData>();
            var destinationDataList = new List<FileData>();
            foreach (var pair in sourceAndDestinationPathPairs)
            {
                var localPath = pair.Item1;
                var remotePath = pair.Item2;

                var localData = CreateFileData(localPath);
                sourceDataList.Add(localData);
                SetUpGetDestinationPath(localData.Path, remotePath);

                var remoteData = CreateFileData(remotePath);
                destinationDataList.Add(remoteData);

                result.Add(Tuple.Create(localData, remoteData));
            }

            SetUpSourceFileData(sourceDataList.ToArray());
            SetUpDestinationFileData(destinationDataList.ToArray());

            return result;
        }

        protected void VerifyFileData(IList<Tuple<FileData, FileData>> expectedFileDataPairs, IEnumerable<FileSynchronizationStatusData> syncData)
        {
            syncData.Count().ShouldBe(expectedFileDataPairs.Count());

            foreach (var expectedPair in expectedFileDataPairs)
            {
                var expectedSource = expectedPair.Item1;
                var expectedDestination = expectedPair.Item2;

                syncData.ShouldContain(d => d.SourceData == expectedSource && d.DestinationData == expectedDestination);
            }
        }

        [Test]
        public async Task ThreeSourceAndDestinationFileMatch()
        {
            var fileData = CreateSourceAndDestinationFiles(
                Tuple.Create("TestFileA", "TestFileA"),
                Tuple.Create("TestFileB", "TestFileB"),
                Tuple.Create("TestFileC", "TestFileC")
            );

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            VerifyFileData(fileData, result);
        }
    }
}