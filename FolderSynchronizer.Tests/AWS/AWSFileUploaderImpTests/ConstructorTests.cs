using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSFileUploaderImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        public enum NullConstructorArg 
        { 
            Logger,
            PathManger,
            ActionTaker,
            FileLister,
            ConfigData,
            SavedFileListRecordUpdater
        }

        [Test]
        [TestCase(NullConstructorArg.Logger)]
        [TestCase(NullConstructorArg.PathManger)]
        [TestCase(NullConstructorArg.ActionTaker)]
        [TestCase(NullConstructorArg.FileLister)]
        [TestCase(NullConstructorArg.ConfigData)]
        [TestCase(NullConstructorArg.SavedFileListRecordUpdater)]
        public void NullArgumentTest(NullConstructorArg nullArg)
        {
            ILogger<AWSFileUploaderImp> logger = nullArg == NullConstructorArg.Logger ? null : GetLogger();
            IAWSPathManager pathManger = nullArg == NullConstructorArg.PathManger ? null : GetPathManger();
            IAWSActionTaker actionTaker = nullArg == NullConstructorArg.ActionTaker ? null : GetActionTaker();
            ILocalFileLister fileLister = nullArg == NullConstructorArg.FileLister ? null : GetLocalFileLister();
            ISavedFileListRecordUpdater savedFileListRecordUpdater = nullArg == NullConstructorArg.SavedFileListRecordUpdater ? null : GetSavedFileListRecordUpdater();

            AWSConfigData configData = nullArg == NullConstructorArg.ConfigData ? null : GetConfigData();

            Should.Throw<ArgumentNullException>(() => new AWSFileUploaderImp(logger, pathManger, actionTaker, fileLister, configData, savedFileListRecordUpdater));
        }

        private ILogger<AWSFileUploaderImp> GetLogger()
        {
            return new Mock<ILogger<AWSFileUploaderImp>>(MockBehavior.Strict).Object;
        }

        private IAWSPathManager GetPathManger()
        {
            return new Mock<IAWSPathManager>(MockBehavior.Strict).Object;
        }

        private IAWSActionTaker GetActionTaker()
        {
            return new Mock<IAWSActionTaker>(MockBehavior.Strict).Object;
        }

        private ILocalFileLister GetLocalFileLister()
        {
            return new Mock<ILocalFileLister>(MockBehavior.Strict).Object;
        }

        private ISavedFileListRecordUpdater GetSavedFileListRecordUpdater()
        {
            return new Mock<ISavedFileListRecordUpdater>(MockBehavior.Strict).Object;
        }

        private AWSConfigData GetConfigData()
        {
            return new AWSConfigData { BucketName = "TestBucket" };
        }
    }
}