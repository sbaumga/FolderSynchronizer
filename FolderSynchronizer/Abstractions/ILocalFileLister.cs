using FolderSynchronizer.Data;

namespace FolderSynchronizer.Abstractions
{
    public interface ILocalFileLister
    {
        IEnumerable<FileData> GetFileDataForFolder(string folderPath);
        IEnumerable<string> GetFilePathsForFolder(string folderPath);
    }
}