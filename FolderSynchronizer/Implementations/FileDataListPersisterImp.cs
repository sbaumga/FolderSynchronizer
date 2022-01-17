using FolderSynchronizer.Abstractions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public class FileDataListPersisterImp : IFileDataListPersister
    {
        private string FilePath { get; }

        private ISerializer Serializer { get; }

        public FileDataListPersisterImp(LocalConfigData configData, ISerializer serializer)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            FilePath = configData.LocalFileListSaveFileLocation;

            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task SaveAsync(IEnumerable<FileData> data)
        {
            var serialized = Serializer.Serialize(data);

            await File.WriteAllTextAsync(FilePath, serialized);
        }

        public async Task<IEnumerable<FileData>> LoadAsync()
        {
            var fileText = await ReadFromFile();
            if (string.IsNullOrEmpty(fileText)) {
                return Enumerable.Empty<FileData>();
            }

            var data = Serializer.Deserialize<IEnumerable<FileData>>(fileText);
            return data;
        }

        private async Task<string> ReadFromFile()
        {
            if (!File.Exists(FilePath))
            {
                return string.Empty;
            }

            var fileText = await File.ReadAllTextAsync(FilePath);
            return fileText;
        }
    }
}