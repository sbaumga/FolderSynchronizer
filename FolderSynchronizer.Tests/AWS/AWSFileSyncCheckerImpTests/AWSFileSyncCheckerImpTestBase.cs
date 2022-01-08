using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using FolderSynchronizer.Data;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileSyncCheckerImpTests
{
    [TestFixture]
    public abstract class AWSFileSyncCheckerImpTestBase
    {
        protected string LocalFolder => "TestFolder";

        protected Mock<ILocalFileLister> LocalFileListerMock { get; set; }
        protected Mock<IAWSFileLister> AWSFileListerMock { get; set; }
        protected Mock<IAWSPathManager> PathManagerMock { get; set; }

        protected AWSFileSyncCheckerImp FileSyncChecker { get; set; }

        [SetUp]
        public void SetUp()
        {
            var configData = new LocalConfigData { LocalFolderName = LocalFolder };

            LocalFileListerMock = new Mock<ILocalFileLister>(MockBehavior.Strict);
            AWSFileListerMock = new Mock<IAWSFileLister>(MockBehavior.Strict);
            PathManagerMock = new Mock<IAWSPathManager>(MockBehavior.Strict);

            FileSyncChecker = new AWSFileSyncCheckerImp(configData, LocalFileListerMock.Object, AWSFileListerMock.Object, PathManagerMock.Object);
        }

        protected void SetUpLocalFileData(params FileData[] data)
        {
            LocalFileListerMock.Setup(x => x.GetFileDataForFolder(LocalFolder)).Returns(data);
        }

        protected void SetUpRemoteFileData(params FileData[] data)
        {
            AWSFileListerMock.Setup(x => x.GetFileDataAsync()).Returns(Task.FromResult(data.AsEnumerable()));
        }

        protected FileData CreateFileData(string name)
        {
            return new FileData
            {
                Path = name,
                LastModifiedDate = DateTime.Now,
            };
        }

        protected void SetUpGetRemotePath(string localPath, string remotePath)
        {
            PathManagerMock.Setup(p => p.GetRemotePath(localPath)).Returns(remotePath);
        }

        protected IList<Tuple<FileData, FileData>> CreateLocalAndRemoteFiles(params Tuple<string, string>[] localAndRemotePathPairs)
        {
            var result = new List<Tuple<FileData, FileData>>();
            
            var localDataList = new List<FileData>();
            var remoteDataList = new List<FileData>();
            foreach (var pair in localAndRemotePathPairs)
            {
                var localPath = pair.Item1;
                var remotePath = pair.Item2;

                var localData = CreateFileData(localPath);
                localDataList.Add(localData);
                SetUpGetRemotePath(localData.Path, remotePath);
                
                var remoteData = CreateFileData(remotePath);
                remoteDataList.Add(remoteData);

                result.Add(Tuple.Create(localData, remoteData));
            }

            SetUpLocalFileData(localDataList.ToArray());
            SetUpRemoteFileData(remoteDataList.ToArray());

            return result;
        }

        protected void VerifyFileData(IList<Tuple<FileData, FileData>> expectedFileDataPairs, IEnumerable<FileSynchronizationStatusData> syncData)
        {
            syncData.Count().ShouldBe(expectedFileDataPairs.Count());

            foreach(var expectedPair in expectedFileDataPairs)
            {
                var expectedLocal = expectedPair.Item1;
                var expectedRemote = expectedPair.Item2;

                syncData.ShouldContain(d => d.SourceData == expectedLocal && d.DestinationData == expectedRemote);
            }
        }
    }
}