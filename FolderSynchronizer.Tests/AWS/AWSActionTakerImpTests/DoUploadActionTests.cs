using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public class DoUploadActionTests : AWSActionTakerImpTestBase<PutObjectRequest, PutObjectResponse>
    {
        protected override Expression<Func<IAmazonS3, Task<PutObjectResponse>>> ClientFuncSetup(PutObjectRequest request)
            => s => s.PutObjectAsync(request, It.IsAny<CancellationToken>());

        protected override async Task<PutObjectResponse> DoAction(PutObjectRequest request)
            => await ActionTaker.DoUploadActionAsync(request);
    }
}