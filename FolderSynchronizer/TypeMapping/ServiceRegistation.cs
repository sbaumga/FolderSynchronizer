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
            RegisterSharedServices(builder);
            RegisterAWSThings(builder);
        }

        private static void RegisterSharedServices(IServiceCollection builder)
        {
            builder.AddTransient(typeof(Abstractions.ILogger<>), typeof(LoggerImp<>));

            builder.AddTransient<ILocalFileLister, LocalFileListerImp>();
            builder.AddTransient<ISerializer, JsonSerializerImp>();
            builder.AddTransient<IJsonSerializer, JsonSerializerImp>();

            builder.AddTransient<ISynchronizationActionDecider, SynchronizationActionDeciderImp>();
            builder.AddTransient<IFileDataCreator, FileDataCreatorImp>();

            builder.AddTransient<ILocalFileDeleter, LocalFileDeleterImp>();

            RegisterSavedFileListServices(builder);
        }

        private static void RegisterSavedFileListServices(IServiceCollection builder)
        {
            builder.AddTransient<IFileDataListPersister, FileDataListPersisterImp>();
            builder.AddTransient<ISavedFileListSyncChecker, SavedFileListSyncCheckerImp>();
            builder.AddTransient<ISavedFileListBulkSynchronizer, SavedFileListBulkSynchronizerImp>();
            builder.AddTransient<ISavedFileListRecordUpdater, SavedFileListRecordUpdaterImp>();
            builder.AddTransient<ISavedFileListRecordDeleter, SavedFileListRecordDeleterImp>();
        }

        private static void RegisterAWSThings(IServiceCollection builder)
        {
            builder.AddTransient<IAWSFileSyncChecker, AWSFileSyncCheckerImp>();
            builder.AddTransient<IAWSBulkFileSynchronizer, AWSBulkFileSynchronizerImp>();
            builder.AddTransient<IAWSFileRenamer, AWSFileRenamerImp>();

            builder.AddTransient<IAWSClientCreator, AWSClientCreatorImp>();
            builder.AddTransient<IAWSPathManager, AWSPathManagerImp>();
            builder.AddTransient<IAWSActionTaker, AWSActionTakerImp>();

            RegisterS3Things(builder);
            RegisterSQSThings(builder);
        }

        private static void RegisterS3Things(IServiceCollection builder)
        {
            builder.AddTransient<IAWSFileLister, AWSFileListerImp>();
            builder.AddTransient<IAWSFileUploader, AWSFileUploaderImp>();
            builder.AddTransient<IAWSFileDeleter, AWSFileDeleterImp>();
            builder.AddTransient<IAWSFileDownloader, AWSFileDownloaderImp>();
        }

        private static void RegisterSQSThings(IServiceCollection builder)
        {
            builder.AddTransient<IAWSSQSMessageDeleter, AWSSQSMessageDeleterImp>();
            builder.AddTransient<IAWSSQSAutomatedS3MessageDeserializer, AWSSQSAutomatedS3MessageDeserializerImp>();
            builder.AddTransient<IAWSSQSMessageConsumer, AWSSQSMessageConsumerImp>();
            builder.AddTransient<IAWSSQSPoller, AWSSQSPollerImp>();
            builder.AddTransient<IAWSSQSKeySanitizer, AWSSQSKeySanitizerImp>();
        }
    }
}