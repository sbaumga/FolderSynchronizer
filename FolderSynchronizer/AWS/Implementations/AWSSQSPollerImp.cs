using Amazon.SQS.Model;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSSQSPollerImp : IAWSSQSPoller
    {
        private string SQSUrl { get; }

        private IAWSActionTaker ActionTaker { get; }

        private IAWSSQSMessageConsumer MessageConsumer { get; }

        public AWSSQSPollerImp(AWSConfigData configData, IAWSActionTaker actionTaker, IAWSSQSMessageConsumer messageConsumer)
        {
            SQSUrl = configData.SQSUrl ?? throw new ArgumentNullException(nameof(configData));
            ActionTaker = actionTaker ?? throw new ArgumentNullException(nameof(actionTaker));
            MessageConsumer = messageConsumer ?? throw new ArgumentNullException(nameof(messageConsumer));
        }

        public async Task<IEnumerable<string>> GetMessagesAsync()
        {
            var request = CreateRequest();
            var response = await ActionTaker.DoSQSPollingAsync(request);

            var result = new List<string>();
            while (response.Messages.Any())
            {
                var messageData = await MessageConsumer.ConsumeMessages(response.Messages);

                result.Concat(messageData.Select(d => $"{d.Key} {(d.Action == Enums.S3Action.Upload ? "Downloaded" : "Deleted")}"));

                response = await ActionTaker.DoSQSPollingAsync(request);
            }

            return result;
        }

        private ReceiveMessageRequest CreateRequest()
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = SQSUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 5,
            };
            return request;
        }
    }
}