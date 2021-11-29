using FolderSynchronizer.AWS.Implementations;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSClientCreatorImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void NullConfigDataTest()
        {
            Should.Throw<ArgumentNullException>(() => new AWSClientCreatorImp(null));
        }
    }
}