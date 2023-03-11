using Amazon.SQS.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Enums;
using FolderSynchronizer.AWS.Exceptions;
using System.Text.RegularExpressions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSSQSAutomatedS3MessageDeserializerImp : IAWSSQSAutomatedS3MessageDeserializer
    {
        private IJsonSerializer Serializer { get; set; }
        private IAWSSQSKeySanitizer KeySanitizer { get; set; }

        public AWSSQSAutomatedS3MessageDeserializerImp(IJsonSerializer serializer, IAWSSQSKeySanitizer keySanitizer)
        {
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            KeySanitizer = keySanitizer ?? throw new ArgumentNullException(nameof(keySanitizer));
        }

        public S3MessageData Deserialize(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var stringToDeserialize = message.Body;
            if (message.Body.Contains("TopicArn"))
            {
                var snsMessage = Serializer.Deserialize<SerializedSQSAutomatedMessageData>(message.Body);
                stringToDeserialize = snsMessage.Message;
            }

            var deserializedData = Serializer.Deserialize<SerializedSQSAutomatedMessageMessageData>(stringToDeserialize);
            var result = ConvertDeserializedDataToS3Data(deserializedData);

            return result;
        }

        private S3MessageData ConvertDeserializedDataToS3Data(SerializedSQSAutomatedMessageMessageData deserializedData)
        {
            if (deserializedData.Records.Count() != 1)
            {
                throw new UnsupportedSQSEventRecordCountException(deserializedData.Records.Count());
            }

            var record = deserializedData.Records.Single();
            var result = new S3MessageData
            {
                BucketName = record.S3.Bucket.Name,
                Timestamp = DateTime.Parse(record.EventTime),
                Key = KeySanitizer.SanitizeKeyFromSQS(record.S3.Object.Key),
                Action = ConvertEventNameToS3Action(record.EventName),
                UserPrincipalId = record.UserIdentity.PrincipalId
            };

            return result;
        }

        public const string UploadEventName = "ObjectCreated:Put";
        public const string DeleteEventName = "ObjectRemoved:Delete";

        private S3Action ConvertEventNameToS3Action(string eventName)
        {
            switch (eventName)
            {
                case UploadEventName:
                    return S3Action.Upload;

                case DeleteEventName:
                    return S3Action.Deletion;

                default:
                    throw new NotImplementedException($"Unknown event name: {eventName}.");
            }
        }
    }
}