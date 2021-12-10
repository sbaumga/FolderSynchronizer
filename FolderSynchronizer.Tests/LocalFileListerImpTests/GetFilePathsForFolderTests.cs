using Shouldly;
using System.Collections.Generic;

namespace FolderSynchronizer.Tests.LocalFileListerImpTests
{
    public class GetFilePathsForFolderTests : LocalFileListerImpTestBase<string>
    {
        protected override IEnumerable<string> PerformTestableFunctionOnPath(string path)
        {
            return FileLister.GetFilePathsForFolder(path);
        }

        protected override void VerifyFilePathExistsInTestableFunctionResult(string filePath, IEnumerable<string> testableFunctionResult)
        {
            testableFunctionResult.ShouldContain(filePath);
        }
    }
}