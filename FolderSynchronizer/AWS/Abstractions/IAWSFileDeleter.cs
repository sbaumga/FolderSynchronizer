namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSFileDeleter
    {
        Task DeleteRemoteFileAsync(string remotePath);

        Task DeleteRemoteFileFromLocalFileAsync(string localPath);
    }
}