
namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSFileSyncChecker
    {
        Task<IEnumerable<FileSynchronizationStatusData>> GetSynchronizationStatusForFilesAsync();
    }
}