using Amazon.Runtime;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.AWS.Implementations;
using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.AWS.AWSClientCreatorImpTests
{
    [TestFixture]
    public abstract class GetClientTestBase<TAmazon>
        where TAmazon : class, IAmazonService
    {
        [Test]
        public void HappyPathTest()
        {
            var clientCreator = CreateAWSClientCreator("TestAccessKey", "TestSecretKey");

            var client = CreateClient(clientCreator);

            client.ShouldNotBeNull();
        }

        protected abstract TAmazon CreateClient(AWSClientCreatorImp clientCreator);

        private AWSClientCreatorImp CreateAWSClientCreator(string accessKey, string secretKey)
        {
            var configData = new AWSConfigData()
            {
                AccessKey = accessKey,
                SecretKey = secretKey
            };

            return new AWSClientCreatorImp(configData);
        }
    }
}