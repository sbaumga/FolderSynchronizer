using Amazon.Runtime;
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
    public abstract class AWSActionTakerImpTestBase<TAmazon, TException, TRequest, TResponse>
        where TAmazon : class, IAmazonService
        where TException : AmazonServiceException
        where TRequest : class, new()
        where TResponse : class, new()
    {
        protected Mock<TAmazon> MockAmazon { get; set; }
        protected Mock<IAWSClientCreator> MockClientCreator { get; set; }
        protected AWSActionTakerImp ActionTaker { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockAmazon = new Mock<TAmazon>(MockBehavior.Strict);

            MockClientCreator = new Mock<IAWSClientCreator>(MockBehavior.Strict);
            SetupClientCreator(MockAmazon);

            ActionTaker = new AWSActionTakerImp(MockClientCreator.Object);
        }

        protected abstract void SetupClientCreator(Mock<TAmazon> mockAmazon);

        [Test]
        public async Task SuccessTest()
        {
            var request = new TRequest();
            var response = new TResponse();
            MockAmazon.Setup(ClientFuncSetup(request)).Returns(Task.FromResult(response));

            var result = await DoAction(request);

            result.ShouldBe(response);
        }

        [Test]
        [TestCase("InvalidAccessKeyId")]
        [TestCase("InvalidSecurity")]
        public void CredentialExceptionThrown(string errorCode)
        {
            var fakeS3Exception = CreateException(errorCode);

            var request = new TRequest();
            MockAmazon.Setup(ClientFuncSetup(request)).Throws(fakeS3Exception);

            CheckThrowsException(request, "Check the provided AWS Credentials.", fakeS3Exception);
        }

        [Test]
        [TestCase(null)]
        [TestCase("UnknownErrorCode")]
        public void UnknownExceptionThrown(string? errorCode)
        {
            var fakeS3Exception = CreateException(errorCode);

            var request = new TRequest();
            MockAmazon.Setup(ClientFuncSetup(request)).Throws(fakeS3Exception);

            CheckThrowsException(request, "Error occurred: " + fakeS3Exception.Message, fakeS3Exception);
        }

        protected TException CreateException(string? errorCode)
        {
            var exception = CreateException();
            exception.ErrorCode = errorCode;
            return exception;
        }

        protected abstract TException CreateException();

        private void CheckThrowsException(TRequest request, string exceptionMessage, Exception innerException)
        {
            Should.Throw<AWSException>(async () => await DoAction(request))
                .ShouldSatisfyAllConditions(
                ex => ex.Message.ShouldBe(exceptionMessage),
                ex => ex.InnerException.ShouldBe(innerException));
        }

        protected abstract Expression<Func<TAmazon, Task<TResponse>>> ClientFuncSetup(TRequest request);

        protected abstract Task<TResponse> DoAction(TRequest request);
    }
}