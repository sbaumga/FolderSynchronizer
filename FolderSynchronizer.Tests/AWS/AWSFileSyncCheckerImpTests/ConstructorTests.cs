using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSFileSyncCheckerImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        public enum NullConstructorArg
        {
            ConfigData,
            LocalFileLister,
            AWSFileLister,
            AWSPathManager
        }

        [Test]
        [TestCase(NullConstructorArg.ConfigData)]
        [TestCase(NullConstructorArg.LocalFileLister)]
        [TestCase(NullConstructorArg.AWSFileLister)]
        [TestCase(NullConstructorArg.AWSPathManager)]
        public void NullArgumentTest(NullConstructorArg nullArg)
        {
            var configData = nullArg == NullConstructorArg.ConfigData ? null : new ConfigData { LocalFolderName = "TestFolder" };
            var localFileLister = nullArg == NullConstructorArg.LocalFileLister ? null : new Mock<ILocalFileLister>(MockBehavior.Strict).Object;
            var awsFileLister = nullArg == NullConstructorArg.AWSFileLister ? null : new Mock<IAWSFileLister>(MockBehavior.Strict).Object;
            var awsPathManager = nullArg == NullConstructorArg.AWSPathManager ? null : new Mock<IAWSPathManager>(MockBehavior.Strict).Object;

            Should.Throw<ArgumentNullException>(() => new AWSFileSyncCheckerImp(configData, localFileLister, awsFileLister, awsPathManager));
        }
    }
}