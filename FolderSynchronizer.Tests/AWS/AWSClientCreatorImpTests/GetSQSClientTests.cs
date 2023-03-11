using Amazon.SQS;
using FolderSynchronizer.AWS.Implementations;

namespace FolderSynchronizer.Tests.AWS.AWSClientCreatorImpTests
{
    public class GetSQSClientTests : GetClientTestBase<IAmazonSQS>
    {
        protected override IAmazonSQS CreateClient(AWSClientCreatorImp clientCreator)
            => clientCreator.GetSQSClient();
    }
}