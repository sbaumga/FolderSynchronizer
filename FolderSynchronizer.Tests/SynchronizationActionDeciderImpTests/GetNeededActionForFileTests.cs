using FolderSynchronizer.Data;
using FolderSynchronizer.Enums;
using FolderSynchronizer.Implementations;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.SynchronizationActionDeciderImpTests
{
    [TestFixture]
    public class GetNeededActionForFileTests
    {
        private SynchronizationActionDeciderImp SyncActionDecider { get; set; }

        [SetUp]
        public void SetUp()
        {
            SyncActionDecider = new SynchronizationActionDeciderImp();
        }

        [Test]
        public void MissingDestinationFileTest()
        {
            var syncData = GetSyncData(null, null);

            DoTest(syncData, FileSynchronizationAction.Upload);
        }

        private FileSynchronizationStatusData GetSyncData(FileData sourceData, FileData destinationData)
        {
            return new FileSynchronizationStatusData { SourceData = sourceData, DestinationData = destinationData };
        }

        private void DoTest(FileSynchronizationStatusData syncData, FileSynchronizationAction expectedResult)
        {
            var action = SyncActionDecider.GetNeededActionForFile(syncData);

            action.ShouldBe(expectedResult);
        }

        [Test]
        public void WithSourceDataAndNoDestinationDataTest()
        {
            var syncData = GetSyncData(null, new FileData());

            DoTest(syncData, FileSynchronizationAction.Delete);
        }

        [Test]
        public void OutOfDateDestinationFile()
        {
            var date = new DateTime(2022, 01, 08);
            var earlierDate = date.AddDays(-1);
            var syncData = GetSyncDataWithLastModifedDates(date, earlierDate);

            DoTest(syncData, FileSynchronizationAction.Upload);
        }

        private FileSynchronizationStatusData GetSyncDataWithLastModifedDates(DateTime sourceDate, DateTime destinationDate)
        {
            var data = GetSyncData(new FileData { LastModifiedDate = sourceDate }, new FileData { LastModifiedDate = destinationDate });
            return data;
        }

        [Test]
        public void OutOfDateSourceFile()
        {
            var date = new DateTime(2022, 01, 08);
            var earlierDate = date.AddDays(-1);
            var syncData = GetSyncDataWithLastModifedDates(earlierDate, date);

            DoTest(syncData, FileSynchronizationAction.None);
        }

        [Test]
        public void MatchingDataFiles()
        {
            var date = new DateTime(2022, 01, 08);
            var syncData = GetSyncDataWithLastModifedDates(date, date);

            DoTest(syncData, FileSynchronizationAction.None);
        }
    }
}