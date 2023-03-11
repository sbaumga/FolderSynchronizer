namespace FolderSynchronizer.Abstractions
{
    public interface ILogger
    {
        void LogInformation(string message);
        void LogError(string error, Exception ex);
    }

    public interface ILogger<TClass> : ILogger
    {
    }
}