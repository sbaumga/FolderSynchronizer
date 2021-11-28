using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.AWS.AWSPathManagerImpTests
{
    public class GetRemotePathTests : AWSPathManagerImpTestBase
    {
        [Test]
        public void FolderNameNotInPath()
        {
            var localPath = FolderName.Substring(1);

            var result = PathManager.GetRemotePath(localPath);

            result.ShouldBe(localPath);
        }

        [Test]
        public void FolderNameInPathButNoSlashes()
        {
            var localPath = FolderName.Substring(1) + "Garbage";

            var result = PathManager.GetRemotePath(localPath);

            result.ShouldBe(localPath);
        }

        [Test]
        public void FolderNameInPathWithForwardSlash()
        {
            var localPath = FolderName.Substring(1) + "/Garbage";

            var result = PathManager.GetRemotePath(localPath);

            result.ShouldBe(localPath);
        }

        [Test]
        public void FolderNameInPathWithBackSlash()
        {
            var localPath = FolderName.Substring(1) + "\\Garbage";

            var result = PathManager.GetRemotePath(localPath);

            result.ShouldBe(FolderName.Substring(1) + "/Garbage");
        }

        [Test]
        public void FolderNameInPathMultipleFilesDeep()
        {
            var localPath = FolderName.Substring(1) + "\\Garbage\\Trash";

            var result = PathManager.GetRemotePath(localPath);

            result.ShouldBe(FolderName.Substring(1) + "/Garbage/Trash");
        }
    }
}