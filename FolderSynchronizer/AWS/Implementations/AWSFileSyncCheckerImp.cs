using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.Data;
using FolderSynchronizer.Implementations;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileSyncCheckerImp : FileSyncCheckerBase, IAWSFileSyncChecker
    {
        private string LocalFolder { get; }
        private ILocalFileLister LocalFileLister { get; }
        private IAWSFileLister AWSFileLister { get; }
        private IAWSPathManager PathManager { get; }

        public AWSFileSyncCheckerImp(LocalConfigData configData, ILocalFileLister localFileLister, IAWSFileLister awsFileLister, IAWSPathManager awsPathManager)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            LocalFolder = configData.LocalFolderName;
            LocalFileLister = localFileLister ?? throw new ArgumentNullException(nameof(localFileLister));
            AWSFileLister = awsFileLister ?? throw new ArgumentNullException(nameof(awsFileLister));
            PathManager = awsPathManager ?? throw new ArgumentNullException(nameof(awsPathManager));
        }

        protected override Task<IEnumerable<FileData>> GetSourceFilesAsync()
        {
            return Task.FromResult(LocalFileLister.GetFileDataForFolder(LocalFolder));
        }

        protected async override Task<IEnumerable<FileData>> GetDestinationFilesAsync()
        {
            return await AWSFileLister.GetFileDataAsync();
        }

        protected override string SourcePathToDestinationPath(string sourcePath)
        {
            return PathManager.GetRemotePath(sourcePath);
        }
    }
}