using Amazon.S3.Model;
using FolderSynchronizer.AWS.Exceptions;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    public class DoDeleteActionTests : AWSActionTakerImpTestBase
    {
        [Test]
        public void DeleteSucceedsTest()
        {
            var request = new DeleteObjectRequest();
            var response = new DeleteObjectResponse();
            MockAmazonS3.Setup(s => s.DeleteObjectAsync(request, It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));

            var result = ActionTaker.DoDeleteAction(request);

            result.ShouldBe(response);
        }

        [Test]
        [TestCase("InvalidAccessKeyId")]
        [TestCase("InvalidSecurity")]
        public void CredentialExceptionThrown(string errorCode)
        {
            var fakeS3Exception = CreateS3Exception(errorCode);

            var request = new DeleteObjectRequest();
            var response = new DeleteObjectResponse();
            MockAmazonS3.Setup(s => s.DeleteObjectAsync(request, It.IsAny<CancellationToken>())).Throws(fakeS3Exception);

            CheckThrowsException(request, "Check the provided AWS Credentials.", fakeS3Exception);
        }

        protected void CheckThrowsException(DeleteObjectRequest request, string exceptionMessage, Exception innerException)
        {
            Should.Throw<AWSException>(() => ActionTaker.DoDeleteAction(request))
                .ShouldSatisfyAllConditions(
                ex => ex.Message.ShouldBe(exceptionMessage),
                ex => ex.InnerException.ShouldBe(innerException));
        }

        [Test]
        [TestCase(null)]
        [TestCase("UnknownErrorCode")]
        public void UnknownExceptionThrown(string? errorCode)
        {
            var fakeS3Exception = CreateS3Exception(errorCode);

            var request = new DeleteObjectRequest();
            var response = new DeleteObjectResponse();
            MockAmazonS3.Setup(s => s.DeleteObjectAsync(request, It.IsAny<CancellationToken>())).Throws(fakeS3Exception);

            CheckThrowsException(request, "Error occurred: " + fakeS3Exception.Message, fakeS3Exception);
        }
    }
}