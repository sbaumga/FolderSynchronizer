using Amazon.SQS.Model;
using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Enums;
using FolderSynchronizer.AWS.Exceptions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSSQSAutomatedS3MessageDeserializerImp : IAWSSQSAutomatedS3MessageDeserializer
    {
        private IJsonSerializer Serializer { get; set; }

        public AWSSQSAutomatedS3MessageDeserializerImp(IJsonSerializer serializer)
        {
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
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
                Key = SanitizeKey(record.S3.Object.Key),
                Action = ConvertEventNameToS3Action(record.EventName),
                UserPrincipalId = record.UserIdentity.PrincipalId
            };

            return result;
        }

        private string SanitizeKey(string key)
        {
            foreach (var pair in ReplacementPairs)
            {
                key = key.Replace(pair.Item1, pair.Item2);
            }

            return key;
        }

        private Tuple<string, string>[] ReplacementPairs => new[]
        {
            Tuple.Create("+", " "),
            Tuple.Create("%26", "&"),
            Tuple.Create("%28", "("),
            Tuple.Create("%29", ")")
        };

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