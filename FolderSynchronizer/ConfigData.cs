namespace MusicLibrarySynchronizer
{
    // Read from appsettings.json/secrets.json
    public class ConfigData
    {
        public string LocalFolderName { get; set; }
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
