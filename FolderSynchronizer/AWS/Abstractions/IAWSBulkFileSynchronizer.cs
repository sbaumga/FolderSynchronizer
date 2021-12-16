
namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSBulkFileSynchronizer
    {
        Task SynchronizeFilesAsync();
    }
}