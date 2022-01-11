using FolderSynchronizer.Implementations;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.SavedFileListRecordDeleterImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void NullArgumentTest()
        {
            Should.Throw<ArgumentNullException>(() => new SavedFileListRecordDeleterImp(null));
        }
    }
}