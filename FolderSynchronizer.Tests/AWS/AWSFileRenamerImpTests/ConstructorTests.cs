using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSFileRenamerImpTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        [TestCase(ConstructorArguments.Uploader)]
        [TestCase(ConstructorArguments.Deleter)]
        [TestCase(ConstructorArguments.PathManager)]
        public void NullArgumentTest(ConstructorArguments nullArg)
        {
            var uploader = nullArg == ConstructorArguments.Uploader ? null : new Mock<IAWSFileUploader>(MockBehavior.Strict).Object;
            var deleter = nullArg == ConstructorArguments.Deleter ? null : new Mock<IAWSFileDeleter>(MockBehavior.Strict).Object;
            var pathManager = nullArg == ConstructorArguments.PathManager ? null : new Mock<IAWSPathManager>(MockBehavior.Strict).Object;

            Should.Throw<ArgumentNullException>(() => new AWSFileRenamerImp(uploader, deleter, pathManager));
        }

        public enum ConstructorArguments
        {
            Uploader,
            Deleter,
            PathManager
        }
    }
}