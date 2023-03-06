namespace FolderSynchronizer.AWS.Exceptions
{
    public class UnsupportedSQSEventRecordCountException : AWSException
    {
        public UnsupportedSQSEventRecordCountException(int recordCount) : base($"Message from SQS contained {recordCount} records. Only one record per message is supported.")
        {
        }
    }
}