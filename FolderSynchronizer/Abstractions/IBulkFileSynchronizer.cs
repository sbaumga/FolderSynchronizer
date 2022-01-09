namespace FolderSynchronizer.Abstractions
{
    public interface IBulkFileSynchronizer
    {
        Task SynchronizeFilesAsync();
    }
}