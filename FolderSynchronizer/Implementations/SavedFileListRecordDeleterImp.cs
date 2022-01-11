using FolderSynchronizer.Abstractions;

namespace FolderSynchronizer.Implementations
{
    public class SavedFileListRecordDeleterImp : ISavedFileListRecordDeleter
    {
        private IFileDataListPersister FileListPersister { get; }

        public SavedFileListRecordDeleterImp(IFileDataListPersister fileListPersister)
        {
            FileListPersister = fileListPersister ?? throw new ArgumentNullException(nameof(fileListPersister));
        }

        public async Task DeleteRecordAsync(string localFilePath)
        {
            var savedData = (await FileListPersister.LoadAsync()).ToList();
            savedData.RemoveAll(d => d.Path == localFilePath);

            await FileListPersister.SaveAsync(savedData);
        }
    }
}