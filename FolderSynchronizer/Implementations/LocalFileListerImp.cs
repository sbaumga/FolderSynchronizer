using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class LocalFileListerImp : ILocalFileLister
    {
        private IFileDataCreator FileDataCreator { get; }

        public LocalFileListerImp(IFileDataCreator fileDataCreator)
        {
            FileDataCreator = fileDataCreator ?? throw new ArgumentNullException(nameof(fileDataCreator));
        }

        public IEnumerable<string> GetFilePathsForFolder(string folderPath)
        {
            var files = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories);
            return files;
        }

        public IEnumerable<FileData> GetFileDataForFolder(string folderPath)
        {
            var paths = GetFilePathsForFolder(folderPath);
            var data = paths.Select(p => FileDataCreator.MakeFileDataFromLocalPath(p));
            return data;
        }
    }
}