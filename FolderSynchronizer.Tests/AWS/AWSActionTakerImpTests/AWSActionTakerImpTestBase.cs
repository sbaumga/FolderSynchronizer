using Amazon.S3;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    [TestFixture]
    public abstract class AWSActionTakerImpTestBase<TRequest, TResponse>
        where TRequest : class, new()
        where TResponse : class, new()
    {
        protected Mock<IAmazonS3> MockAmazonS3 { get; set; }
        protected Mock<IAWSClientCreator> MockClientCreator { get; set; }
        protected AWSActionTakerImp ActionTaker { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockAmazonS3 = new Mock<IAmazonS3>(MockBehavior.Strict);

            MockClientCreator = new Mock<IAWSClientCreator>(MockBehavior.Strict);
            MockClientCreator.Setup(c => c.GetS3Client()).Returns(MockAmazonS3.Object);

            ActionTaker = new AWSActionTakerImp(MockClientCreator.Object);
        }

        protected AmazonS3Exception CreateS3Exception(string? errorCode)
        {
            var exception = new AmazonS3Exception("TestException")
            {
                ErrorCode = errorCode
            };
            return exception;
        }

        [Test]
        public async Task SuccessTest()
        {
            var request = new TRequest();
            var response = new TResponse();
            MockAmazonS3.Setup(ClientFuncSetup(request)).Returns(Task.FromResult(response));

            var result = await DoAction(request);

            result.ShouldBe(response);
        }

        [Test]
        [TestCase("InvalidAccessKeyId")]
        [TestCase("InvalidSecurity")]
        public void CredentialExceptionThrown(string errorCode)
        {
            var fakeS3Exception = CreateS3Exception(errorCode);

            var request = new TRequest();
            MockAmazonS3.Setup(ClientFuncSetup(request)).Throws(fakeS3Exception);

            CheckThrowsException(request, "Check the provided AWS Credentials.", fakeS3Exception);
        }

        [Test]
        [TestCase(null)]
        [TestCase("UnknownErrorCode")]
        public void UnknownExceptionThrown(string? errorCode)
        {
            var fakeS3Exception = CreateS3Exception(errorCode);

            var request = new TRequest();
            MockAmazonS3.Setup(ClientFuncSetup(request)).Throws(fakeS3Exception);

            CheckThrowsException(request, "Error occurred: " + fakeS3Exception.Message, fakeS3Exception);
        }

        private void CheckThrowsException(TRequest request, string exceptionMessage, Exception innerException)
        {
            Should.Throw<AWSException>(async () => await DoAction(request))
                .ShouldSatisfyAllConditions(
                ex => ex.Message.ShouldBe(exceptionMessage),
                ex => ex.InnerException.ShouldBe(innerException));
        }

        protected abstract Expression<Func<IAmazonS3, Task<TResponse>>> ClientFuncSetup(TRequest request);

        protected abstract Task<TResponse> DoAction(TRequest request);
    }
}