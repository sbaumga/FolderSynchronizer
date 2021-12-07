namespace FolderSynchronizer.Tests.AWS.AWSFileDeleterImpTests
{
    public class DeleteRemoteFileAsyncTests : FileDeletionTestBase
    {
        protected override void DeletionFunction(string path)
        {
            Deleter.DeleteRemoteFileAsync(path).Wait();
        }

        protected override string DeletionPath => "Garbage";
        protected override string FolderPath => DeletionPath;
    }
}