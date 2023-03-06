namespace FolderSynchronizer.AWS.Data
{
    public class AWSConfigData
    {
        public string BucketName { get; set; }
        public string SQSUrl { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string PrincipalId { get; set; }
    }
}
