using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.SavedFileListRecordUpdaterImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        public enum NullConstructorArg
        {
            DataCreator,
            Persister
        }

        [Test]
        [TestCase(NullConstructorArg.DataCreator)]
        [TestCase(NullConstructorArg.Persister)]
        public void NullArgumentTest(NullConstructorArg nullArg)
        {
            var creator = nullArg == NullConstructorArg.DataCreator ? null : new Mock<IFileDataCreator>(MockBehavior.Strict).Object;
            var persister = nullArg == NullConstructorArg.Persister ? null : new Mock<IFileDataListPersister>(MockBehavior.Strict).Object;

            Should.Throw<ArgumentNullException>(() => new SavedFileListRecordUpdaterImp(creator, persister));
        }
    }
}