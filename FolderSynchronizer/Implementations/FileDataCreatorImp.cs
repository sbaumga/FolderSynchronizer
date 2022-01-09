using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class FileDataCreatorImp : IFileDataCreator
    {
        public FileData MakeFileDataFromLocalPath(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            var data = new FileData
            {
                Path = path,
                LastModifiedDate = File.GetLastWriteTimeUtc(path)
            };
            return data;
        }
    }
}