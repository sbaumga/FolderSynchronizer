using Amazon.SQS;
using Moq;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public abstract class AWSSQSActionTakerImpTestBase<TRequest, TResponse> : AWSActionTakerImpTestBase<IAmazonSQS, AmazonSQSException, TRequest, TResponse>
        where TRequest : class, new()
        where TResponse : class, new()
    {
        protected override void SetupClientCreator(Mock<IAmazonSQS> mockAmazon)
        {
            MockClientCreator.Setup(c => c.GetSQSClient()).Returns(mockAmazon.Object);
        }

        protected override AmazonSQSException CreateException()
            => new("TestException");
    }
}