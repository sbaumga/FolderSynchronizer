using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.FileDataListPersisterImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void NullConfigDataTest()
        {
            var serializer = new Mock<ISerializer>(MockBehavior.Strict);

            Should.Throw<ArgumentNullException>(() => new FileDataListPersisterImp(null, serializer.Object));
        }

        [Test]
        public void NullSerializerTest()
        {
            var configData = new LocalConfigData();

            Should.Throw<ArgumentNullException>(() => new FileDataListPersisterImp(configData, null));
        }
    }
}