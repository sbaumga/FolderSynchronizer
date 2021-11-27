using FolderSynchronizer.AWS;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;

namespace FolderSynchronizer.TypeMapping
{
    public static class ServiceRegistation
    {
        public static void Register(IServiceCollection builder)
        {
            builder.AddTransient<FolderWatcher>();

            builder.AddTransient<LocalFileLister>();

            RegisterAWSThings(builder);
        }

        private static void RegisterAWSThings(IServiceCollection builder)
        {
            builder.AddTransient<AWSFileManager>();
            builder.AddTransient<AWSFileLister>();
            builder.AddTransient<AWSFileUploader>();
            builder.AddTransient<AWSFileDeleter>();
            builder.AddTransient<AWSFileRenamer>();
            builder.AddTransient<AWSFileSyncChecker>();
            builder.AddTransient<AWSBulkFileSynchronizer>();

            builder.AddTransient<AWSClientCreator>();
            builder.AddTransient<AWSPathManager>();
            builder.AddTransient<AWSActionTaker, AWSActionTakerImp>();
        }
    }
}
