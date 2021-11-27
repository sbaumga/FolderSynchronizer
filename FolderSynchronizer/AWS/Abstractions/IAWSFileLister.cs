namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSFileLister
    {
        Task<IEnumerable<string>> ListFilesAsync();

        Task<IEnumerable<string>> ListFilteredFilesAsync(string startOfPath);

        Task<IEnumerable<FileData>> GetFileDataAsync();
    }
}