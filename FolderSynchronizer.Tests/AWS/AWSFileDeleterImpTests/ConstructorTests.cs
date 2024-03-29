﻿using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSFileDeleterImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        public enum NullConstructorArg
        {
            PathManger,
            ActionTaker,
            FileLister,
            ConfigData,
            SavedFileListRecordDeleter,
            Logger
        }

        [Test]
        [TestCase(NullConstructorArg.PathManger)]
        [TestCase(NullConstructorArg.ActionTaker)]
        [TestCase(NullConstructorArg.FileLister)]
        [TestCase(NullConstructorArg.ConfigData)]
        [TestCase(NullConstructorArg.SavedFileListRecordDeleter)]
        [TestCase(NullConstructorArg.Logger)]
        public void ArgumentTest(NullConstructorArg nullArg)
        {
            var pathManger = nullArg == NullConstructorArg.PathManger ? null : GetPathManger();
            var actionTaker = nullArg == NullConstructorArg.ActionTaker ? null : GetActionTaker();
            var fileLister = nullArg == NullConstructorArg.FileLister ? null : GetAWSFileLister();
            var savedFileListRecordDeleter = nullArg == NullConstructorArg.SavedFileListRecordDeleter ? null : GetSavedFileListRecordDeleter();
            var logger = nullArg == NullConstructorArg.Logger ? null : GetLogger();

            var configData = nullArg == NullConstructorArg.ConfigData ? null : GetConfigData();

            Should.Throw<ArgumentNullException>(() => new AWSFileDeleterImp(pathManger, fileLister, actionTaker, configData, savedFileListRecordDeleter, logger));
        }

        private IAWSPathManager GetPathManger()
        {
            return new Mock<IAWSPathManager>(MockBehavior.Strict).Object;
        }

        private IAWSActionTaker GetActionTaker()
        {
            return new Mock<IAWSActionTaker>(MockBehavior.Strict).Object;
        }

        private IAWSFileLister GetAWSFileLister()
        {
            return new Mock<IAWSFileLister>(MockBehavior.Strict).Object;
        }

        private ISavedFileListRecordDeleter GetSavedFileListRecordDeleter()
        {
            return new Mock<ISavedFileListRecordDeleter>(MockBehavior.Strict).Object;
        }

        private ILogger<AWSFileDeleterImp> GetLogger()
        {
            return new Mock<ILogger<AWSFileDeleterImp>>(MockBehavior.Strict).Object;
        }

        private AWSConfigData GetConfigData()
        {
            return new AWSConfigData { BucketName = "TestBucket" };
        }
    }
}