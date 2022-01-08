﻿using Amazon.S3.Model;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSFileDeleterImp : IAWSFileDeleter
    {
        private IAWSPathManager PathManager { get; }
        private IAWSFileLister FileLister { get; }
        private IAWSActionTaker ActionTaker { get; }

        private string BucketName { get; }

        public AWSFileDeleterImp(IAWSPathManager pathManager, IAWSFileLister fileLister, IAWSActionTaker actionTaker, AWSConfigData configData)
        {
            PathManager = pathManager ?? throw new ArgumentNullException(nameof(pathManager));
            FileLister = fileLister ?? throw new ArgumentNullException(nameof(fileLister));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));

            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }
            BucketName = configData.BucketName;
        }

        public async Task DeleteRemoteFileFromLocalFileAsync(string localPath)
        {
            var remotePath = PathManager.GetRemotePath(localPath);
            await DeleteRemoteFileAsync(remotePath);
        }

        public async Task DeleteRemoteFileAsync(string remotePath)
        {
            if (PathManager.IsPathFile(remotePath))
            {
                await DeleteFile(remotePath);
            }
            else
            {
                await DeleteFolder(remotePath);
            }
        }

        private async Task DeleteFile(string remotePath)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = remotePath,
            };

            var response = ActionTaker.DoDeleteAction(deleteRequest);
            if (!response.HttpStatusCode.HasFlag(System.Net.HttpStatusCode.OK))
            {
                throw new Exception(response.ToString());
            }
        }

        private async Task DeleteFolder(string remotePath)
        {
            var filesInFolder = await FileLister.ListFilteredFilesAsync(remotePath);

            var deletionTasks = filesInFolder.Select(f => DeleteFile(f));
            await Task.WhenAll(deletionTasks);
        }
    }
}