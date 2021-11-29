using Amazon.S3;
using Amazon.S3.Model;
using FolderSynchronizer.AWS.Exceptions;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileUploaderImpTests
{
    public class UploadFileAsyncTests : AWSFileUploaderImpTestBase
    {
        [Test]
        public async Task UploadingFolderTest()
        {
            SetUpIsPathFile(false);

            Should.NotThrow(async () => await Uploader.UploadFileAsync("Garbage"));
        }

        public async Task UploadReturnsErrorCodeTest()
        {
            SetUpIsPathFile(true);

            var response = new PutObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.BadRequest
            };
            MockActionTaker.Setup(t => t.DoS3Action(It.IsAny<Func<AmazonS3Client, PutObjectResponse>>())).Returns(response);

            Should.Throw<AWSFileUploadException>(async () => await Uploader.UploadFileAsync("Garbage/Trash.txt"));
        }

        public async Task UploadSucceedsTest()
        {
            SetUpIsPathFile(true);

            var response = new PutObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };
            MockActionTaker.Setup(t => t.DoS3Action(It.IsAny<Func<AmazonS3Client, PutObjectResponse>>())).Returns(response);

            Should.NotThrow(async () => await Uploader.UploadFileAsync("Garbage/Trash.txt"));
        }
    }
}