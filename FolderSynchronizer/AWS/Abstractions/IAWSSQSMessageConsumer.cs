using Amazon.SQS.Model;
using FolderSynchronizer.AWS.Data;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSSQSMessageConsumer
    {
        Task<IEnumerable<S3MessageData>> ConsumeMessages(IList<Message> messages);
    }
}