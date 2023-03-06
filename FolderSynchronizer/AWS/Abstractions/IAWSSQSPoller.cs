namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSSQSPoller
    {
        Task<IEnumerable<string>> GetMessagesAsync();
    }
}