using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class SavedFileListRecordUpdaterImp : ISavedFileListRecordUpdater
    {
        private IFileDataCreator FileDataCreator { get; }
        private IFileDataListPersister FileListPersister { get; }

        public SavedFileListRecordUpdaterImp(IFileDataCreator fileDataCreator, IFileDataListPersister fileListPersister)
        {
            FileDataCreator = fileDataCreator ?? throw new ArgumentNullException(nameof(fileDataCreator));
            FileListPersister = fileListPersister ?? throw new ArgumentNullException(nameof(fileListPersister));
        }

        public async Task AddOrUpdateRecordAsync(string localFilePath)
        {
            var savedDataTask = FileListPersister.LoadAsync();

            var fileData = FileDataCreator.MakeFileDataFromLocalPath(localFilePath);

            var savedData = (await savedDataTask).ToList();
            RemoveDataIfPresent(savedData, localFilePath);

            savedData.Add(fileData);

            await FileListPersister.SaveAsync(savedData);
        }

        private void RemoveDataIfPresent(IList<FileData> fileData, string localPath)
        {
            var existingData = fileData.SingleOrDefault(d => d.Path == localPath);
            if (existingData != null)
            {
                fileData.Remove(existingData);
            }
        }
    }
}