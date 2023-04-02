using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Implementations;
using FolderSynchronizer.Data;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSFileDownloaderImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        public enum NullConstructorArg
        {
            LocalConfig,
            AwsConfig,
            ActionTaker,
            SavedFileListRecordUpdater
        }

        [TestCase(NullConstructorArg.LocalConfig)]
        [TestCase(NullConstructorArg.AwsConfig)]
        [TestCase(NullConstructorArg.ActionTaker)]
        [TestCase(NullConstructorArg.SavedFileListRecordUpdater)]
        public void NullArgumentTest(NullConstructorArg nullArg)
        {
            var localConfig = nullArg == NullConstructorArg.LocalConfig ? null : new LocalConfigData { LocalFolderName = "Test Folder"};
            var awsConfig = nullArg == NullConstructorArg.AwsConfig ? null : new AWSConfigData { BucketName = "Test Bucket"};
            var actionTaker = nullArg == NullConstructorArg.ActionTaker ? null : new Mock<IAWSActionTaker>(MockBehavior.Strict).Object;
            var recordUpdater = nullArg == NullConstructorArg.SavedFileListRecordUpdater ? null : new Mock<ISavedFileListRecordUpdater>(MockBehavior.Strict).Object;

            Should.Throw<ArgumentNullException>(() => new AWSFileDownloaderImp(localConfig, awsConfig, actionTaker, recordUpdater));
        }
    }
}