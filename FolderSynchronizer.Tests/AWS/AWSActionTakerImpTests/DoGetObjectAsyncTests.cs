using Amazon.S3;
using Amazon.S3.Model;
using FolderSynchronizer.AWS.Exceptions;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public class DoGetObjectAsyncTests : AWSS3ActionTakerImpTestBase<GetObjectRequest, GetObjectResponse>
    {
        protected override Expression<Func<IAmazonS3, Task<GetObjectResponse>>> ClientFuncSetup(GetObjectRequest request)
            => s => s.GetObjectAsync(request, It.IsAny<CancellationToken>());

        protected override async Task<GetObjectResponse> DoAction(GetObjectRequest request)
            => await ActionTaker.DoGetObjectAsync(request);

        [Test]
        public void ObjectDoesNotExistTest()
        {
            var fakeS3Exception = CreateException("NoSuchKey");

            var key = "testKey";
            var request = new GetObjectRequest
            {
                Key = key,
            };
            MockAmazon.Setup(ClientFuncSetup(request)).Throws(fakeS3Exception);

            Should.Throw<FileDoesNotExistInBucketException>(async () => await DoAction(request))
                .ShouldSatisfyAllConditions(
                ex => ex.Message.ShouldBe($"\"{key}\" does not exist in the specified s3 bucket."));
        }
    }
}