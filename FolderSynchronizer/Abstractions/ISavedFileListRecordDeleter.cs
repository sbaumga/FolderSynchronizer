namespace FolderSynchronizer.Abstractions
{
    public interface ISavedFileListRecordDeleter
    {
        Task DeleteRecordAsync(string localFilePath);
    }
}