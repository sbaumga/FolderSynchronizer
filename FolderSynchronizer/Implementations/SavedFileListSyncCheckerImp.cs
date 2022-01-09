using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class SavedFileListSyncCheckerImp : FileSyncCheckerBaseImp, ISavedFileListSyncChecker
    {
        private string FolderPath { get; }

        private ILocalFileLister FileLister { get; }
        private IFileDataListPersister FileDataListPersister { get; }

        public SavedFileListSyncCheckerImp(LocalConfigData configData, ILocalFileLister fileLister, IFileDataListPersister fileDataListPersister)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            FolderPath = configData.LocalFolderName;

            FileLister = fileLister ?? throw new ArgumentNullException(nameof(fileLister));
            FileDataListPersister = fileDataListPersister ?? throw new ArgumentNullException(nameof(fileDataListPersister));
        }

        protected override Task<IEnumerable<FileData>> GetSourceFilesAsync() => Task.FromResult(FileLister.GetFileDataForFolder(FolderPath));

        protected override Task<IEnumerable<FileData>> GetDestinationFilesAsync() => FileDataListPersister.LoadAsync();

        protected override string SourcePathToDestinationPath(string sourcePath)
        {
            return sourcePath;
        }
    }
}