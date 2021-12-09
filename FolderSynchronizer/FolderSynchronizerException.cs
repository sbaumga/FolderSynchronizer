namespace FolderSynchronizer
{
    public class FolderSynchronizerException : Exception
    {
        public FolderSynchronizerException()
        {
        }

        public FolderSynchronizerException(string message) : base(message)
        {
        }

        public FolderSynchronizerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}