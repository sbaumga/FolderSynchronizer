using FolderSynchronizer.Abstractions;
using FolderSynchronizer.AWS;
using FolderSynchronizer.AWS.Abstractions;
using FolderSynchronizer.AWS.Implementations;
using FolderSynchronizer.Implementations;

namespace FolderSynchronizer.TypeMapping
{
    public static class ServiceRegistation
    {
        public static void Register(IServiceCollection builder)
        {
            builder.AddTransient<FolderWatcher>();

            builder.AddTransient<ILocalFileLister, LocalFileListerImp>();

            RegisterAWSThings(builder);
        }

        private static void RegisterAWSThings(IServiceCollection builder)
        {
            builder.AddTransient<AWSFileManager>();
            builder.AddTransient<AWSFileSyncChecker>();
            builder.AddTransient<AWSBulkFileSynchronizer>();
            builder.AddTransient<AWSFileRenamer>();

            builder.AddTransient<IAWSFileLister, AWSFileListerImp>();
            builder.AddTransient<AWSFileUploader>();
            builder.AddTransient<AWSFileDeleter>();

            builder.AddTransient<IAWSClientCreator, AWSClientCreatorImp>();
            builder.AddTransient<IAWSPathManager, AWSPathManagerImp>();
            builder.AddTransient<IAWSActionTaker, AWSActionTakerImp>();
        }
    }
}
