namespace FolderSynchronizer.Implementations
{
    public class LoggerImp<TClass> : Abstractions.ILogger<TClass>
    {
        private ILogger<TClass> Logger { get; }

        public LoggerImp(ILogger<TClass> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogInformation(string message)
        {
            Logger.LogInformation(message);
        }
    }
}