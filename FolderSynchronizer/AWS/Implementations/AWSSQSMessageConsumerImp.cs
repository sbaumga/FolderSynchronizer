using Amazon.SQS.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Enums;
using FolderSynchronizer.AWS.Exceptions;
using ILogger = FolderSynchronizer.Abstractions.ILogger;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSSQSMessageConsumerImp : IAWSSQSMessageConsumer
    {
        private string UserPrincipalId { get; }

        private IAWSSQSAutomatedS3MessageDeserializer MessageDeserializer { get; }
        private IAWSSQSMessageDeleter MessageDeleter { get; }
        private IAWSFileDownloader FileDownloader { get; }
        private ILocalFileDeleter FileDeleter { get; }
        private ILogger Logger { get; }
        
        public AWSSQSMessageConsumerImp(AWSConfigData configData, IAWSSQSAutomatedS3MessageDeserializer messageDeserializer, IAWSSQSMessageDeleter messageDeleter, IAWSFileDownloader fileDownloader, ILocalFileDeleter fileDeleter, FolderSynchronizer.Abstractions.ILogger<AWSSQSMessageConsumerImp> logger)
        {
            UserPrincipalId = configData?.PrincipalId ?? throw new ArgumentNullException(nameof(configData));

            MessageDeserializer = messageDeserializer ?? throw new ArgumentNullException(nameof(messageDeserializer));
            MessageDeleter = messageDeleter ?? throw new ArgumentNullException(nameof(messageDeleter));
            FileDownloader = fileDownloader ?? throw new ArgumentNullException(nameof(fileDownloader));
            FileDeleter = fileDeleter ?? throw new ArgumentNullException(nameof(fileDeleter));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<S3MessageData>> ConsumeMessages(IList<Message> messages)
        {
            var messageData = new List<S3MessageData>();

            foreach (var message in messages)
            {
                var data = await ConsumeMessage(message);
                if (data != null)
                {
                    messageData.Add(data);
                }
            }

            return messageData;
        }

        private async Task<S3MessageData> ConsumeMessage(Message message)
        {
            S3MessageData messageData = null;
            try
            {
                messageData = MessageDeserializer.Deserialize(message);
                await TakeActionFromMessage(messageData);
            }
            catch (NotAutomatedMessageException)
            {
                // If not an automated message, can't do anything with it,
                // but we'll delete it so we don't have to deal with it again later.
            }
            catch (Exception ex)
            {
                // TODO: log to a more permanent place
                // TODO: provide way to preserve message and move past it.
                Logger.LogError($"Error consuming message: \"{message.Body}\". Deleting and continuing...", ex);
            }

            await MessageDeleter.Delete(message);

            return messageData;
        }

        private async Task TakeActionFromMessage(S3MessageData data)
        {
            var isSameUser = data.UserPrincipalId == UserPrincipalId;
            if (isSameUser)
            {
                return;
            }

            switch (data.Action)
            {
                case S3Action.Upload:
                    await DownloadFile(data.Key);
                    break;

                case S3Action.Deletion:
                    await FileDeleter.DeleteFile(data.Key);
                    break;

                default:
                    throw new NotImplementedException($"Unsupported {nameof(S3Action)} of {data.Action}");
            }
        }

        private async Task DownloadFile(string fileKey)
        {
            try
            {
                await FileDownloader.DownloadFile(fileKey);
            }
            catch (FileDoesNotExistInBucketException ex)
            {
                // File doesn't exist in bucket, can't download so just continue
                Logger.LogInformation(ex.Message);
            }
        }
    }
}