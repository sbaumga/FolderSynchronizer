using FolderSynchronizer.Data;

namespace FolderSynchronizer.Abstractions
{
    public interface IFileSynchronizationChecker
    {
        Task<IEnumerable<FileSynchronizationStatusData>> GetSynchronizationStatusForFilesAsync();
    }
}