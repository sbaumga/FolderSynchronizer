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

            builder.AddTransient(typeof(Abstractions.ILogger<>), typeof(LoggerImp<>));

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
            builder.AddTransient<IAWSFileUploader, AWSFileUploaderImp>();
            builder.AddTransient<IAWSFileDeleter, AWSFileDeleterImp>();

            builder.AddTransient<IAWSClientCreator, AWSClientCreatorImp>();
            builder.AddTransient<IAWSPathManager, AWSPathManagerImp>();
            builder.AddTransient<IAWSActionTaker, AWSActionTakerImp>();
        }
    }
}
