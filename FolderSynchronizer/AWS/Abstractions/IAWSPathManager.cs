namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSPathManager
    {
        string GetRemotePath(string localPath);
        bool IsPathFile(string path);
    }
}