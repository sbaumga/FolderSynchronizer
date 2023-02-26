using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Data;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSClientCreatorImp : IAWSClientCreator
    {
        private AWSCredentials AWSCredentials { get; }

        public AWSClientCreatorImp(AWSConfigData configData)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            AWSCredentials = new BasicAWSCredentials(configData.AccessKey, configData.SecretKey);
        }

        public IAmazonS3 GetS3Client()
        {
            var client = new AmazonS3Client(AWSCredentials, RegionEndpoint.CACentral1);
            return client;
        }
    }
}
