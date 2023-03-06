namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSFileDownloader
    {
        Task DownloadFile(string fileKey);
    }
}