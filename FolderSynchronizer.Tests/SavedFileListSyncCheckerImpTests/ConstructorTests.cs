using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.SavedFileListSyncCheckerImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        public enum NullConstructorArg
        {
            ConfigData,
            FileLister,
            Persister
        }

        [Test]
        [TestCase(NullConstructorArg.ConfigData)]
        [TestCase(NullConstructorArg.FileLister)]
        [TestCase(NullConstructorArg.Persister)]
        public void NullArgumentTest(NullConstructorArg nullArg)
        {
            var configData = nullArg == NullConstructorArg.ConfigData ? null : new LocalConfigData();
            var fileLister = nullArg == NullConstructorArg.FileLister ? null : new Mock<ILocalFileLister>(MockBehavior.Strict).Object;
            var persister = nullArg == NullConstructorArg.Persister ? null : new Mock<IFileDataListPersister>(MockBehavior.Strict).Object;

            Should.Throw<ArgumentNullException>(() => new SavedFileListSyncCheckerImp(configData, fileLister, persister));
        }
    }
}