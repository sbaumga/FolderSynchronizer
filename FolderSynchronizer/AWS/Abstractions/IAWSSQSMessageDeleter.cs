using Amazon.SQS.Model;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSSQSMessageDeleter
    {
        Task Delete(Message message);
    }
}