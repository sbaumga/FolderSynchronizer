using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class LocalFileDeleterImp : ILocalFileDeleter
    {
        private string LocalFolderPath { get; }

        private ISavedFileListRecordDeleter SavedFileListRecordDeleter { get; }

        public LocalFileDeleterImp(LocalConfigData configData, ISavedFileListRecordDeleter savedFileListRecordDeleter)
        {
            LocalFolderPath = configData?.LocalFolderName ?? throw new ArgumentNullException(nameof(configData));
            SavedFileListRecordDeleter = savedFileListRecordDeleter ?? throw new ArgumentNullException(nameof(configData));
        }

        public async Task DeleteFile(string fileKey)
        {
            var path = Path.Combine(LocalFolderPath, fileKey);
            File.Delete(path);

            await SavedFileListRecordDeleter.DeleteRecordAsync(path);
        }
    }
}