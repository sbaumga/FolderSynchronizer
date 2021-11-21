using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace FolderSynchronizer.AWS
{
    public class AWSClientCreator
    {
        private AWSCredentials AWSCredentials { get; }

        public AWSClientCreator(ConfigData configData)
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
