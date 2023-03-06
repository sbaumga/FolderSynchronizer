using Amazon.SQS.Model;

namespace FolderSynchronizer.AWS.Exceptions
{
    public class NotAutomatedMessageException : AWSException
    {
        public NotAutomatedMessageException(Message message, Exception innerException) :
            base($"The following message is not an automated message: {message.Body}", innerException)
        { }
    }
}