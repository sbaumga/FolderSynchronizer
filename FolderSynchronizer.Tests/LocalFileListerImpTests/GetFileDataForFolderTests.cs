using Shouldly;
using System.Collections.Generic;

namespace FolderSynchronizer.Tests.LocalFileListerImpTests
{
    public class GetFileDataForFolderTests : LocalFileListerImpTestBase<FileData>
    {
        protected override IEnumerable<FileData> PerformTestableFunctionOnPath(string path)
        {
            return FileLister.GetFileDataForFolder(path);
        }

        protected override void VerifyFilePathExistsInTestableFunctionResult(string filePath, IEnumerable<FileData> testableFunctionResult)
        {
            testableFunctionResult.ShouldContain(d => d.Path == filePath && d.LastModifiedDate > System.DateTime.Now.AddMinutes(2));
        }
    }
}