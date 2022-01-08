using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSFileListerImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void NullActionTaker()
        {
            var configData = new AWSConfigData { BucketName = "TestBucket" };

            Should.Throw<ArgumentNullException>(() => new AWSFileListerImp(null, configData));
        }

        [Test]
        public void NullConfigData()
        {
            var actionTaker = new Mock<IAWSActionTaker>();

            Should.Throw<ArgumentNullException>(() => new AWSFileListerImp(actionTaker.Object, null));
        }
    }
}