namespace FolderSynchronizer
{
    public class FolderSynchronizerException : Exception
    {
        public FolderSynchronizerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}