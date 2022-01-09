using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSBulkFileSynchronizerImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        public enum NullConstructorArg
        {
            FileSyncChecker,
            Uploader,
            Deleter,
            SyncActionDecider
        }

        [Test]
        [TestCase(NullConstructorArg.FileSyncChecker)]
        [TestCase(NullConstructorArg.Uploader)]
        [TestCase(NullConstructorArg.Deleter)]
        [TestCase(NullConstructorArg.SyncActionDecider)]
        public void NullArgumentTest(NullConstructorArg nullArg)
        {
            var syncChecker = nullArg == NullConstructorArg.FileSyncChecker ? null : new Mock<IAWSFileSyncChecker>(MockBehavior.Strict).Object;
            var uploader = nullArg == NullConstructorArg.Uploader ? null : new Mock<IAWSFileUploader>(MockBehavior.Strict).Object;
            var deleter = nullArg == NullConstructorArg.Deleter ? null : new Mock<IAWSFileDeleter>(MockBehavior.Strict).Object;
            var syncActionDecider = nullArg == NullConstructorArg.SyncActionDecider ? null : new Mock<ISynchronizationActionDecider>(MockBehavior.Strict).Object;

            Should.Throw<ArgumentNullException>(() => new AWSBulkFileSynchronizerImp(syncChecker, uploader, deleter, syncActionDecider));
        }
    }
}