using FolderSynchronizer.AWS.Implementations;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void NullConfigData()
        {
            Should.Throw<ArgumentNullException>(() => new AWSActionTakerImp(null));
        }
    }
}