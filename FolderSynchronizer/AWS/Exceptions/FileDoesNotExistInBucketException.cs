namespace FolderSynchronizer.AWS.Exceptions
{
    public class FileDoesNotExistInBucketException : AWSException
    {
        public FileDoesNotExistInBucketException(string fileKey) : base($"\"{fileKey}\" does not exist in the specified s3 bucket.")
        {
        }
    }
}