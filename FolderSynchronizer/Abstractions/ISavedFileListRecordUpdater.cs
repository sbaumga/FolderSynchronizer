
namespace FolderSynchronizer.Abstractions
{
    public interface ISavedFileListRecordUpdater
    {
        Task AddOrUpdateRecordAsync(string localFilePath);
    }
}