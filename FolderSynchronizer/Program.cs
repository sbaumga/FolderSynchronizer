using FolderSynchronizer;
using FolderSynchronizer.AWS.Data;
using FolderSynchronizer.Data;
using FolderSynchronizer.TypeMapping;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) => {
        builder.SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
            .AddUserSecrets<LocalConfigData>()
            .AddUserSecrets<AWSConfigData>();
    })
    .ConfigureServices((context, builder) => {
        builder.AddTransient(provider => context.Configuration.GetSection("LocalConfigData").Get<LocalConfigData>());
        builder.AddTransient(provider => context.Configuration.GetSection("AWSConfigData").Get<AWSConfigData>());
        ServiceRegistation.Register(builder);

        builder.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
