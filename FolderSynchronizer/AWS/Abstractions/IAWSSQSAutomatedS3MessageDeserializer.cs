using Amazon.SQS.Model;
using FolderSynchronizer.AWS.Data;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSSQSAutomatedS3MessageDeserializer
    {
        S3MessageData Deserialize(Message message);
    }
}