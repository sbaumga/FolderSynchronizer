using Amazon.S3;
using FolderSynchronizer.AWS.Implementations;
using NUnit.Framework;

namespace FolderSynchronizer.Tests.AWS.AWSClientCreatorImpTests
{
    public class GetS3ClientTests : GetClientTestBase<IAmazonS3>
    {
        protected override IAmazonS3 CreateClient(AWSClientCreatorImp clientCreator)
            => clientCreator.GetS3Client();
    }
}