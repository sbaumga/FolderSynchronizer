
namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSFileRenamer
    {
        Task RenameFileAsync(string oldLocalPath, string newLocalPath);
    }
}