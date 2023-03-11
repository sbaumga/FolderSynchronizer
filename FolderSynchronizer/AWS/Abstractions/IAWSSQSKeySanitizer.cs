namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSSQSKeySanitizer
    {
        string SanitizeKeyFromSQS(string key);
    }
}