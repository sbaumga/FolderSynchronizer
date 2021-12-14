using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileSyncCheckerImp : IAWSFileSyncChecker
    {
        private string LocalFolder { get; }
        private ILocalFileLister LocalFileLister { get; }
        private IAWSFileLister AWSFileLister { get; }
        private IAWSPathManager PathManager { get; }

        public AWSFileSyncCheckerImp(ConfigData configData, ILocalFileLister localFileLister, IAWSFileLister awsFileLister, IAWSPathManager awsPathManager)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            LocalFolder = configData.LocalFolderName;
            LocalFileLister = localFileLister ?? throw new ArgumentNullException(nameof(localFileLister));
            AWSFileLister = awsFileLister ?? throw new ArgumentNullException(nameof(awsFileLister));
            PathManager = awsPathManager ?? throw new ArgumentNullException(nameof(awsPathManager));
        }

        public async Task<IEnumerable<FileSynchronizationStatusData>> GetSynchronizationStatusForFilesAsync()
        {
            var localFiles = LocalFileLister.GetFileDataForFolder(LocalFolder);
            var remoteFiles = (await AWSFileLister.GetFileDataAsync()).ToList();

            var syncData = new List<FileSynchronizationStatusData>();
            foreach (var localFile in localFiles)
            {
                syncData.Add(CreateFileSynchronizationStatusDataForLocalFile(localFile, remoteFiles));
            }

            syncData.AddRange(FindMissingRemoteFiles(syncData, remoteFiles));

            return syncData;
        }

        private FileSynchronizationStatusData CreateFileSynchronizationStatusDataForLocalFile(FileData localFile, IList<FileData> remoteFiles)
        {
            var expectedRemotePathForLocalPath = PathManager.GetRemotePath(localFile.Path);
            var matchingRemoteFile = remoteFiles.SingleOrDefault(file => file.Path == expectedRemotePathForLocalPath);

            var data = new FileSynchronizationStatusData
            {
                LocalData = localFile,
                RemoteData = matchingRemoteFile
            };

            return data;
        }

        private IList<FileSynchronizationStatusData> FindMissingRemoteFiles(IList<FileSynchronizationStatusData> localFileSyncData, IList<FileData> remoteFiles)
        {
            var missingFiles = remoteFiles.Where(remote => !localFileSyncData.Any(d => d.RemoteData == remote));
            var syncData = missingFiles.Select(m => new FileSynchronizationStatusData
            {
                LocalData = null,
                RemoteData = m
            }).ToList();

            return syncData;
        }
    }
}