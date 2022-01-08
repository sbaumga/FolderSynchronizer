using FolderSynchronizer.Data;

namespace FolderSynchronizer.Abstractions
{
    public interface IFileDataListPersister
    {
        Task SaveAsync(IEnumerable<FileData> data);
        Task<IEnumerable<FileData>> LoadAsync();
    }
}