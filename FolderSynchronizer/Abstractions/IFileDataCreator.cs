using FolderSynchronizer.Data;

namespace FolderSynchronizer.Abstractions
{
    public interface IFileDataCreator
    {
        FileData MakeFileDataFromLocalPath(string path);
    }
}