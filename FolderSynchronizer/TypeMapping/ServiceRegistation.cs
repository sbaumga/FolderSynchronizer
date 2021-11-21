using FolderSynchronizer.AWS;

namespace FolderSynchronizer.TypeMapping
{
    public static class ServiceRegistation
    {
        public static void Register(IServiceCollection builder)
        {
            builder.AddTransient<FolderWatcher>();

            builder.AddTransient<AWSFileManager>();
            builder.AddTransient<AWSFileLister>();
            builder.AddTransient<AWSFileUploader>();
            builder.AddTransient<AWSFileDeleter>();
            builder.AddTransient<AWSFileRenamer>();

            builder.AddTransient<AWSClientCreator>();
            builder.AddTransient<AWSPathManager>();
            builder.AddTransient<AWSActionTaker>();
        }
    }
}
