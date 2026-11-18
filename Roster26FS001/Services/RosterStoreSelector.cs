using Microsoft.Extensions.Hosting;

namespace Roster26FS001.Services;

public enum StorageMode
{
    InMemory,
    AzureMySqlInApp
}

public sealed class RosterStoreSelector
{
    public StorageMode Select(IHostEnvironment environment, string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(environment);

        return IsAzure(environment)
            ? StorageMode.AzureMySqlInApp
            : StorageMode.InMemory;
    }

    public IRosterStore Create(
        IHostEnvironment environment,
        string? connectionString,
        string? portOverride,
        InMemoryRosterStore inMemoryStore,
        AzureInAppConnectionStringNormalizer normalizer,
        ILogger<AzureMySqlRosterStore> logger)
    {
        if (Select(environment, connectionString) is not StorageMode.AzureMySqlInApp)
        {
            return inMemoryStore;
        }

        var connection = normalizer.Normalize(connectionString, portOverride);
        return new AzureMySqlRosterStore(connection, logger);
    }

    private static bool IsAzure(IHostEnvironment environment) =>
        string.Equals(environment.EnvironmentName, "Azure", StringComparison.OrdinalIgnoreCase)
        || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"))
        || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
}