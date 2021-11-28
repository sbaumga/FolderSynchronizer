using FolderSynchronizer.Abstractions;

namespace FolderSynchronizer.Implementations
{
    public class LocalFileListerImp : ILocalFileLister
    {
        public IEnumerable<string> GetFilePathsForFolder(string folderPath)
        {
            var files = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories);
            return files;
        }

        public IEnumerable<FileData> GetFileDataForFolder(string folderPath)
        {
            var paths = GetFilePathsForFolder(folderPath);
            var data = paths.Select(p => MakeFileDataFromPath(p));
            return data;
        }

        private FileData MakeFileDataFromPath(string path)
        {
            var data = new FileData
            {
                Path = path,
                LastModifiedDate = File.GetLastWriteTimeUtc(path)
            };
            return data;
        }
    }
}