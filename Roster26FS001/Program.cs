using Roster26FS001.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<InMemoryRosterStore>();
builder.Services.AddSingleton<AzureInAppConnectionStringNormalizer>();
builder.Services.AddSingleton<AzureMySqlConnectionSource>();
builder.Services.AddSingleton<RosterStoreSelector>();
builder.Services.AddSingleton<IRosterStore>(serviceProvider =>
{
    var selector = serviceProvider.GetRequiredService<RosterStoreSelector>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
    var connectionString = serviceProvider
        .GetRequiredService<AzureMySqlConnectionSource>()
        .Resolve(configuration);
    var portOverride = configuration["MYSQL_PORT"]
        ?? configuration["WEBSITE_MYSQL_PORT"]
        ?? Environment.GetEnvironmentVariable("MYSQL_PORT")
        ?? Environment.GetEnvironmentVariable("WEBSITE_MYSQL_PORT");
    var selectedMode = selector.Select(environment, connectionString);
    var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("RosterStoreSelection");

    logger.LogInformation(
        "Roster storage selected. StorageMode={StorageMode} HostingEnvironment={HostingEnvironment} ConnectionConfigured={ConnectionConfigured} PortOverrideConfigured={PortOverrideConfigured}",
        selectedMode,
        environment.EnvironmentName,
        !string.IsNullOrWhiteSpace(connectionString),
        !string.IsNullOrWhiteSpace(portOverride));

    return selector.Create(
        environment,
        connectionString,
        portOverride,
        serviceProvider.GetRequiredService<InMemoryRosterStore>(),
        serviceProvider.GetRequiredService<AzureInAppConnectionStringNormalizer>(),
        serviceProvider.GetRequiredService<ILogger<AzureMySqlRosterStore>>());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
