using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.AWS.AWSPathManagerImpTests
{
    public class IsPathFileTests : AWSPathManagerImpTestBase
    {
        [Test]
        public void FolderTest()
        {
            var path = @"~\AWS";

            var result = PathManager.IsPathFile(path);

            result.ShouldBe(false);
        }

        [Test]
        public void FileTest()
        {
            var path = @"~\AWS\AWSPathManagerImpTests\IsPathFile.cs";

            var result = PathManager.IsPathFile(path);

            result.ShouldBe(true);
        }
    }
}