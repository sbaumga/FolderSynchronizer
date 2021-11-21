namespace FolderSynchronizer.AWS
{
    public class AWSPathManager
    {
        public string SanitizeRemotePath(string path)
        {
            return path.Replace(@"\", "/");
        }

        public bool IsPathFile(string path)
        {
            var extension = Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension);
        }
    }
}
