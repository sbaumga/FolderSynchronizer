using Amazon.S3;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Exceptions;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace FolderSynchronizer.Tests.AWS.AWSActionTakerImpTests
{
    [TestFixture]
    public class DoS3ActionAsyncTests
    {
        private Mock<IAWSClientCreator> MockClientCreator { get; set; }
        private AWSActionTakerImp? ActionTaker { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockClientCreator = new Mock<IAWSClientCreator>(MockBehavior.Strict);
            MockClientCreator.Setup(c => c.GetS3Client()).Returns(new AmazonS3Client());

            ActionTaker = new AWSActionTakerImp(MockClientCreator.Object);
        }

        [TearDown]
        public void TearDown()
        {
            ActionTaker = null;
        }

        [Test]
        public void ActionSucceedsTest()
        {
            const int expectedResult = 1;
            static int fakeS3Action(AmazonS3Client client) => expectedResult;

            var result = ActionTaker.DoS3Action(fakeS3Action);

            result.ShouldBe(expectedResult, "Did not return expected result from action.");
        }


        [Test]
        [TestCase("InvalidAccessKeyId")]
        [TestCase("InvalidSecurity")]
        public void CredentialExceptionThrown(string errorCode)
        {
            var fakeS3Exception = CreateS3Exception(errorCode);
            int fakeS3Action(AmazonS3Client _) => throw fakeS3Exception;

            CheckThrowsException(fakeS3Action, "Check the provided AWS Credentials.", fakeS3Exception);
        }

        private AmazonS3Exception CreateS3Exception(string? errorCode)
        {
            var exception = new AmazonS3Exception("TestException")
            {
                ErrorCode = errorCode
            };
            return exception;
        }

        private void CheckThrowsException<TResponse>(Func<AmazonS3Client, TResponse> fakeS3Action, string exceptionMessage, Exception innerException)
        {
            Should.Throw<AWSException>(() => ActionTaker.DoS3Action(fakeS3Action))
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
            int fakeS3Action(AmazonS3Client _) => throw fakeS3Exception;

            CheckThrowsException(fakeS3Action, "Error occurred: " + fakeS3Exception.Message, fakeS3Exception);
        }
    }
}