using FolderSynchronizer.Data;

namespace FolderSynchronizer.Implementations
{
    public abstract class FileSyncCheckerBase
    {
        public async Task<IEnumerable<FileSynchronizationStatusData>> GetSynchronizationStatusForFilesAsync()
        {
            var sourceFiles = await GetSourceFilesAsync();
            var destinationFiles = (await GetDestinationFilesAsync()).ToList();

            var syncData = new List<FileSynchronizationStatusData>();
            foreach (var localFile in sourceFiles)
            {
                syncData.Add(CreateFileSynchronizationStatusDataForLocalFile(localFile, destinationFiles));
            }

            syncData.AddRange(FindMissingDestinationFiles(syncData, destinationFiles));

            return syncData;
        }

        protected abstract Task<IEnumerable<FileData>> GetSourceFilesAsync();

        protected abstract Task<IEnumerable<FileData>> GetDestinationFilesAsync();

        private FileSynchronizationStatusData CreateFileSynchronizationStatusDataForLocalFile(FileData sourceFile, IList<FileData> destinationFiles)
        {
            var expectedDestinationPathForSourcePath = SourcePathToDestinationPath(sourceFile.Path);
            var matchingDestinationFile = destinationFiles.SingleOrDefault(file => file.Path == expectedDestinationPathForSourcePath);

            var data = new FileSynchronizationStatusData
            {
                SourceData = sourceFile,
                DestinationData = matchingDestinationFile
            };

            return data;
        }

        protected abstract string SourcePathToDestinationPath(string sourcePath);

        private IList<FileSynchronizationStatusData> FindMissingDestinationFiles(IList<FileSynchronizationStatusData> sourceFileSyncData, IList<FileData> destinationFiles)
        {
            var missingFiles = destinationFiles.Where(destinationFile => !sourceFileSyncData.Any(d => d.DestinationData == destinationFile));
            var syncData = missingFiles.Select(m => new FileSynchronizationStatusData
            {
                SourceData = null,
                DestinationData = m
            }).ToList();

            return syncData;
        }
    }
}