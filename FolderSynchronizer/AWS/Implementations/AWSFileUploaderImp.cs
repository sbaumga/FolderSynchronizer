﻿using Amazon.S3.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.Data;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileUploaderImp : IAWSFileUploader
    {
        private FolderSynchronizer.Abstractions.ILogger Logger { get; }

        private IAWSPathManager PathManager { get; }
        private IAWSActionTaker ActionTaker { get; }
        private ILocalFileLister LocalFileLister { get; }
        private ISavedFileListRecordUpdater SavedFileListRecordUpdater { get; }

        private string BucketName { get; }
        private string MachineName { get; }

        public AWSFileUploaderImp(
            FolderSynchronizer.Abstractions.ILogger<AWSFileUploaderImp> logger,
            IAWSPathManager pathManager,
            IAWSActionTaker actionTaker,
            ILocalFileLister localFileLister,
            AWSConfigData awsConfigData,
            LocalConfigData localConfigData,
            ISavedFileListRecordUpdater savedFileListRecordUpdater)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            PathManager = pathManager ?? throw new ArgumentNullException(nameof(pathManager));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));
            LocalFileLister = localFileLister ?? throw new ArgumentNullException(nameof(localFileLister));
            SavedFileListRecordUpdater = savedFileListRecordUpdater ?? throw new ArgumentNullException(nameof(savedFileListRecordUpdater));

            BucketName = awsConfigData?.BucketName ?? throw new ArgumentNullException(nameof(awsConfigData)); ;
            MachineName = localConfigData?.MachineName ?? throw new ArgumentNullException(nameof(localConfigData));
        }

        public async Task UploadFileAsync(string localPath)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            if (!PathManager.IsPathFile(localPath))
            {
                return;
            }

            var remotePath = PathManager.GetRemotePath(localPath);

            LogUploadFileMessage(localPath, remotePath);

            var putRequest = GetPutRequest(localPath, remotePath);

            var response = ActionTaker.DoUploadAction(putRequest);
            if (!response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK))
            {
                throw new AWSFileUploadException($"Upload of file \"{localPath}\" failed: {response}");
            }

            await SavedFileListRecordUpdater.AddOrUpdateRecordAsync(localPath);

            LogUploadFileCompleteMessage(localPath, remotePath);
        }

        private void LogUploadFileMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"Uploading file {localPath} to {remotePath}");
        }

        private PutObjectRequest GetPutRequest(string localPath, string remotePath)
        {
            var mimeType = GetMimeType(localPath);

            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
                FilePath = localPath,
                ContentType = mimeType,
            };
            putRequest.Metadata.Add(nameof(LocalConfigData.MachineName), MachineName);

            return putRequest;
        }

        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var mimeType = MimeTypes.MimeTypeMap.GetMimeType(extension);
            return mimeType;
        }

        private void LogUploadFileCompleteMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"File {localPath} successfully uploaded to {remotePath}");
        }

        public async Task UploadFolderAsync(string localPath)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            var remotePath = PathManager.GetRemotePath(localPath);
            LogUploadFolderMessage(localPath, remotePath);

            var files = LocalFileLister.GetFilePathsForFolder(localPath);
            var uploadTasks = files.Select(f =>
            {
                return UploadFileAsync(f);
            });
            await Task.WhenAll(uploadTasks);

            LogUploadFolderCompleteMessage(localPath, remotePath);
        }

        private void LogUploadFolderMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"Uploading folder {localPath} to {remotePath}");
        }

        private void LogUploadFolderCompleteMessage(string localPath, string remotePath)
        {
            Logger.LogInformation($"Folder {localPath} successfully uploaded to {remotePath}");
        }
    }
}