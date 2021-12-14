using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileSyncCheckerImpTests
{
    public class GetSynchronizationStatusForFilesAsyncTests : AWSFileSyncCheckerImpTestBase
    {
        [Test]
        public async Task NoFilesTest()
        {
            SetUpLocalFileData();
            SetUpRemoteFileData();

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            result.ShouldBeEmpty();
        }

        [Test]
        public async Task LocalFileWithNoRemote()
        {
            var localData = CreateFileData("TestFile");
            SetUpGetRemotePath(localData.Path, "TestFileRemote");
            SetUpLocalFileData(localData);

            SetUpRemoteFileData();

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            result.Count().ShouldBe(1);

            var syncData = result.Single();

            syncData.LocalData.ShouldBe(localData);
            syncData.RemoteData.ShouldBeNull();
        }

        [Test]
        public async Task RemoteFileWithNoLocal()
        {
            SetUpLocalFileData();

            var remoteData = CreateFileData("TestFile");
            SetUpRemoteFileData(remoteData);

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            result.Count().ShouldBe(1);

            var syncData = result.Single();

            syncData.LocalData.ShouldBeNull();
            syncData.RemoteData.ShouldBe(remoteData);
        }

        [Test]
        public async Task OneLocalAndRemoteFileMatch()
        {
            var fileData = CreateLocalAndRemoteFiles(Tuple.Create("TestFile", "TestFileRemote"));

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            VerifyFileData(fileData, result);
        }

        [Test]
        public async Task ThreeLocalAndRemoteFileMatch()
        {
            var fileData = CreateLocalAndRemoteFiles(
                Tuple.Create("TestFileA", "TestFileARemote"),
                Tuple.Create("TestFileB", "TestFileBRemote"),
                Tuple.Create("TestFileC", "TestFileCRemote")
            );

            var result = await FileSyncChecker.GetSynchronizationStatusForFilesAsync();

            VerifyFileData(fileData, result);
        }
    }
}