using Amazon.SQS.Model;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSSQSMessageDeleterImp : IAWSSQSMessageDeleter
    {
        private string SQSUrl { get; }

        private IAWSActionTaker ActionTaker { get; }

        public AWSSQSMessageDeleterImp(AWSConfigData configData, IAWSActionTaker actionTaker)
        {
            SQSUrl = configData?.SQSUrl ?? throw new ArgumentNullException(nameof(configData));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));
        }

        public async Task Delete(Message message)
        {
            var request = CreateDeleteMessageRequest(message);

            var result = await ActionTaker.DoSQSMessageDeletionAsync(request);
        }

        private DeleteMessageRequest CreateDeleteMessageRequest(Message message)
        {
            var request = new DeleteMessageRequest
            {
                QueueUrl = SQSUrl,
                ReceiptHandle = message.ReceiptHandle
            };

            return request;
        }
    }
}