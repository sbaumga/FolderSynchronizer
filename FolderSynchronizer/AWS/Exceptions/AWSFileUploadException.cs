namespace FolderSynchronizer.AWS.Exceptions
{
    public class AWSFileUploadException : AWSException
    {
        public AWSFileUploadException() 
        {
        }

        public AWSFileUploadException(string message) : base(message)
        {
        }
    }
}