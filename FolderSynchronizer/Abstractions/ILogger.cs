namespace FolderSynchronizer.Abstractions
{
    public interface ILogger
    {
        void LogInformation(string message);
    }

    public interface ILogger<TClass> : ILogger
    { 
    }
}