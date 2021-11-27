namespace FolderSynchronizer.AWS
{
    public class AWSPathManager
    {
        public string FolderName { get; }

        public AWSPathManager(ConfigData configData)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            FolderName = configData.LocalFolderName;
        }

        public string GetRemotePath(string localPath)
        {
            var remotePath = localPath.Replace(FolderName + @"\", "");
            remotePath = SanitizeRemotePath(remotePath);
            return remotePath;
        }

        private string SanitizeRemotePath(string path)
        {
            return path.Replace(@"\", "/");
        }

        // TODO: move somewhere not AWS related
        public bool IsPathFile(string path)
        {
            var extension = Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension);
        }
    }
}
