using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public class DoDeleteActionTests : AWSActionTakerImpTestBase<DeleteObjectRequest, DeleteObjectResponse>
    {
        protected override Expression<Func<IAmazonS3, Task<DeleteObjectResponse>>> ClientFuncSetup(DeleteObjectRequest request)
            => s => s.DeleteObjectAsync(request, It.IsAny<CancellationToken>());

        protected override async Task<DeleteObjectResponse> DoAction(DeleteObjectRequest request)
            => await ActionTaker.DoDeleteActionAsync(request);
    }
}