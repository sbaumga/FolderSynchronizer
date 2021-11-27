using FolderSynchronizer.AWS.Implementations;
using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.AWS.AWSClientCreatorImpTests
{
    [TestFixture]
    public class GetS3ClientTests
    {
        [Test]
        public void HappyPathTest()
        {
            var clientCreator = CreateAWSClientCreator("TestAccessKey", "TestSecretKey");

            var client = clientCreator.GetS3Client();

            client.ShouldNotBeNull();
        }

        private AWSClientCreatorImp CreateAWSClientCreator(string accessKey, string secretKey)
        {
            var configData = new ConfigData()
            {
                AccessKey = accessKey,
                SecretKey = secretKey
            };

            return new AWSClientCreatorImp(configData);
        }
    }
}