using MusicLibrarySynchronizer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) => {
        builder.SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
            .AddUserSecrets<ConfigData>();
    })
    .ConfigureServices((context, builder) => {
        // TODO: use the proper .AddConfiguration method?
        builder.AddTransient(provider => context.Configuration.GetSection("ConfigData").Get<ConfigData>());

        builder.AddHostedService<Worker>();
        builder.AddTransient<FolderWatcher>();
        builder.AddTransient<AWSFileManager>();
    })
    .Build();

await host.RunAsync();
