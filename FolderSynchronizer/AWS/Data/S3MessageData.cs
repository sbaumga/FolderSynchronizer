using FolderSynchronizer.AWS.Enums;

namespace FolderSynchronizer.AWS.Data
{
    public class S3MessageData
    {
        public string UserPrincipalId { get; set; }

        public string BucketName { get; set; }

        public DateTime? Timestamp { get; set; }

        public S3Action Action { get; set; }

        public string Key { get; set; }
    }
}