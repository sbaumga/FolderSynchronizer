using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public class DoSQSMessageDeletionAsyncTests : AWSSQSActionTakerImpTestBase<DeleteMessageRequest, DeleteMessageResponse>
    {
        protected override Expression<Func<IAmazonSQS, Task<DeleteMessageResponse>>> ClientFuncSetup(DeleteMessageRequest request)
            => s => s.DeleteMessageAsync(request, It.IsAny<CancellationToken>());

        protected override Task<DeleteMessageResponse> DoAction(DeleteMessageRequest request)
            => this.ActionTaker.DoSQSMessageDeletionAsync(request);
    }
}