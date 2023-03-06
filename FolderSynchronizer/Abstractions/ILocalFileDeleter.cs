namespace FolderSynchronizer.Abstractions
{
    public interface ILocalFileDeleter
    {
        Task DeleteFile(string fileKey);
    }
}