﻿using Amazon.S3.Model;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Implementations;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FolderSynchronizer.Tests.AWS.AWSFileListerImpTests
{
    [TestFixture]
    public abstract class AWSFileListerImpTestBase
    {
        protected Mock<IAWSActionTaker> MockActionTaker { get; set; }

        protected AWSConfigData ConfigData { get; } = new AWSConfigData { BucketName = "TestBucket" };

        protected AWSFileListerImp FileLister { get; set; }

        [SetUp]
        public void SetUp()
        {
            MockActionTaker = new Mock<IAWSActionTaker>(MockBehavior.Strict);

            FileLister = new AWSFileListerImp(MockActionTaker.Object, ConfigData);
        }

        protected void SetUpMockActionTakerToReturnResponseWithS3Objects(params S3Object[] s3Objects)
        {
            var fakeResponse = new ListObjectsV2Response
            {
                S3Objects = new List<S3Object>(s3Objects)
            };

            MockActionTaker.Setup(s => s.DoListActionAsync(It.IsAny<ListObjectsV2Request>())).Returns(Task.FromResult(fakeResponse));
        }

        protected S3Object CreateS3Object(string key, DateTime? lastModifiedDate = null)
        {
            var s3Object = new S3Object
            {
                Key = key,
                LastModified = lastModifiedDate ?? DateTime.UtcNow,
            };
            return s3Object;
        }
    }
}