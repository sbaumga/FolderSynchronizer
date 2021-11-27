namespace FolderSynchronizer.AWS.Exceptions
{
    public class AWSException : FolderSynchronizerException
    {
        public AWSException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}