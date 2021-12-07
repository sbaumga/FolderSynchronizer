namespace FolderSynchronizer.Tests.AWS.AWSFileDeleterImpTests
{
    public class DeleteRemoteFileFromLocalFileAsyncTests : FileDeletionTestBase
    {
        private string RemotePath = "Trash";
        protected override string DeletionPath => RemotePath;
        protected override string FolderPath => RemotePath;

        public override void SetUp()
        {
            base.SetUp();

            SetUpGetRemotePath(DeletionPath, RemotePath);
        }

        protected override void DeletionFunction(string path)
        {
            Deleter.DeleteRemoteFileFromLocalFileAsync(path).Wait();
        }

        protected override void AdditionalMultipleFileFolderFilePathsSetUp()
        {
            foreach(var filePath in MultipleFileFolderFilePaths)
            {
                SetUpGetRemotePath(filePath, filePath);
            }
        }
    }
}