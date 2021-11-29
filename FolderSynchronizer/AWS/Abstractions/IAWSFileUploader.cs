
namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSFileUploader
    {
        Task UploadFileAsync(string localPath);
        Task UploadFolderAsync(string localPath);
    }
}