using Amazon.S3.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileDownloaderImp : IAWSFileDownloader
    {
        private string LocalFolderPath { get; }

        private string BucketName { get; }

        private IAWSActionTaker ActionTaker { get; }

        private ISavedFileListRecordUpdater SavedFileListUpdater { get; }

        public AWSFileDownloaderImp(LocalConfigData localConfigData, AWSConfigData awsConfigData, IAWSActionTaker actionTaker, ISavedFileListRecordUpdater savedFileListUpdater)
        {
            LocalFolderPath = localConfigData?.LocalFolderName ?? throw new ArgumentNullException(nameof(localConfigData));

            BucketName = awsConfigData?.BucketName ?? throw new ArgumentNullException(nameof(awsConfigData));

            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));
            SavedFileListUpdater = savedFileListUpdater ?? throw new ArgumentNullException(nameof(savedFileListUpdater));
        }

        public async Task DownloadFile(string fileKey)
        {
            try
            {
                var fileContents = await GetFileBytes(fileKey);

                var path = SaveFile(fileKey, fileContents);

                await SavedFileListUpdater.AddOrUpdateRecordAsync(path);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<byte[]> GetFileBytes(string fileKey)
        {
            try
            {
                var request = CreateGetObjectRequest(fileKey);
                var response = await ActionTaker.DoGetObjectAsync(request);

                using (var responseStream = response.ResponseStream)
                {
                    using (var bReader = new BinaryReader(responseStream))
                    {
                        var bytes = bReader.ReadBytes((int)responseStream.Length);
                        return bytes;
                    }
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(e => e.Message == "The specified key does not exist."))
                {
                    throw new FileDoesNotExistInBucketException(fileKey);
                }

                throw;
            }
        }

        private GetObjectRequest CreateGetObjectRequest(string fileKey)
            => new GetObjectRequest
            {
                BucketName = BucketName,
                Key = fileKey
            };

        private string SaveFile(string fileKey, byte[] fileContents)
        {
            var path = Path.Combine(LocalFolderPath, fileKey);
            var file = new FileInfo(path);
            file.Directory.Create();
            File.WriteAllBytes(path, fileContents);

            return path;
        }
    }
}