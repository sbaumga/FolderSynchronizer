using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    internal class DoSQSPollingAsyncTests : AWSSQSActionTakerImpTestBase<ReceiveMessageRequest, ReceiveMessageResponse>
    {
        protected override Expression<Func<IAmazonSQS, Task<ReceiveMessageResponse>>> ClientFuncSetup(ReceiveMessageRequest request)
            => s => s.ReceiveMessageAsync(request, It.IsAny<CancellationToken>());

        protected override Task<ReceiveMessageResponse> DoAction(ReceiveMessageRequest request)
            => this.ActionTaker.DoSQSPollingAsync(request);
    }
}