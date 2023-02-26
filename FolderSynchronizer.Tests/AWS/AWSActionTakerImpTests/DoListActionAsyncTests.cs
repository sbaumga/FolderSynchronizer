using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public class DoListActionAsyncTests : AWSActionTakerImpTestBase<ListObjectsV2Request, ListObjectsV2Response>
    {
        protected override Expression<Func<IAmazonS3, Task<ListObjectsV2Response>>> ClientFuncSetup(ListObjectsV2Request request)
            => s => s.ListObjectsV2Async(request, It.IsAny<CancellationToken>());

        protected override async Task<ListObjectsV2Response> DoAction(ListObjectsV2Request request)
            => await ActionTaker.DoListActionAsync(request);
    }
}