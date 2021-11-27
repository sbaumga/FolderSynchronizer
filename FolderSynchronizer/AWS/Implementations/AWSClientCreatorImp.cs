using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using FolderSynchronizer.AWS.Abstractions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSClientCreatorImp : IAWSClientCreator
    {
        private AWSCredentials AWSCredentials { get; }

        public AWSClientCreatorImp(ConfigData configData)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            AWSCredentials = new BasicAWSCredentials(configData.AccessKey, configData.SecretKey);
        }

        public AmazonS3Client GetS3Client()
        {
            var client = new AmazonS3Client(AWSCredentials, RegionEndpoint.CACentral1);
            return client;
        }
    }
}
