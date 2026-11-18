using MySqlConnector;
using Roster26FS001.Data;
using Roster26FS001.Models;

namespace Roster26FS001.Services;

public sealed class AzureMySqlRosterStore : IRosterStore
{
    private readonly string connectionString;
    private readonly ILogger<AzureMySqlRosterStore> logger;
    private readonly SemaphoreSlim schemaGate = new(1, 1);
    private bool schemaInitialized;

    public AzureMySqlRosterStore(
        AzureInAppConnection connection,
        ILogger<AzureMySqlRosterStore> logger)
    {
        connectionString = connection.ToConnectionString();
        this.logger = logger;
    }

    public void Add(RosterEntry entry) => AddAsync(entry).GetAwaiter().GetResult();

    public IReadOnlyList<RosterEntry> GetSnapshot() => GetAllAsync().GetAwaiter().GetResult();

    public async Task AddAsync(RosterEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);
        await EnsureSchemaAsync(cancellationToken);

        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO RosterEntries
                    (FirstName, LastName, Major, FavoriteKeyboardShortcut, ShortcutUsage, CreatedAtUtc)
                VALUES
                    (@firstName, @lastName, @major, @favoriteKeyboardShortcut, @shortcutUsage, @createdAtUtc);
                """;
            command.Parameters.AddWithValue("@firstName", entry.FirstName);
            command.Parameters.AddWithValue("@lastName", entry.LastName);
            command.Parameters.AddWithValue("@major", entry.Major);
            command.Parameters.AddWithValue("@favoriteKeyboardShortcut", entry.FavoriteKeyboardShortcut);
            command.Parameters.AddWithValue("@shortcutUsage", entry.ShortcutUsage);
            command.Parameters.AddWithValue("@createdAtUtc", entry.CreatedAtUtc == default ? DateTime.UtcNow : entry.CreatedAtUtc);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is MySqlException or IOException or TimeoutException)
        {
            logger.LogError("Roster persistence operation failed. Operation={Operation} StorageMode={StorageMode} FailureCategory={FailureCategory} ExceptionType={ExceptionType}", "add", StorageMode.AzureMySqlInApp, "write", exception.GetType().Name);
            throw new InvalidOperationException("Roster persistence is unavailable.", exception);
        }
    }

    public async Task<IReadOnlyList<RosterEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);

        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT Id, FirstName, LastName, Major, FavoriteKeyboardShortcut, ShortcutUsage, CreatedAtUtc
                FROM RosterEntries
                ORDER BY CreatedAtUtc, Id;
                """;
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var entries = new List<RosterEntry>();
            while (await reader.ReadAsync(cancellationToken))
            {
                entries.Add(new RosterEntry
                {
                    Id = reader.GetInt64(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Major = reader.GetString(3),
                    FavoriteKeyboardShortcut = reader.GetString(4),
                    ShortcutUsage = reader.GetString(5),
                    CreatedAtUtc = DateTime.SpecifyKind(reader.GetDateTime(6), DateTimeKind.Utc)
                });
            }

            return entries;
        }
        catch (Exception exception) when (exception is MySqlException or IOException or TimeoutException)
        {
            logger.LogError("Roster persistence operation failed. Operation={Operation} StorageMode={StorageMode} FailureCategory={FailureCategory} ExceptionType={ExceptionType}", "read", StorageMode.AzureMySqlInApp, "read", exception.GetType().Name);
            throw new InvalidOperationException("Roster persistence is unavailable.", exception);
        }
    }

    private async Task EnsureSchemaAsync(CancellationToken cancellationToken)
    {
        if (schemaInitialized)
        {
            return;
        }

        await schemaGate.WaitAsync(cancellationToken);
        try
        {
            if (schemaInitialized)
            {
                return;
            }

            try
            {
                await using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = MySqlRosterSchema.CreateTable;
                await command.ExecuteNonQueryAsync(cancellationToken);
                schemaInitialized = true;
            }
            catch (Exception exception) when (exception is MySqlException or IOException or TimeoutException)
            {
                LogPersistenceFailure("schema-initialization", "database", exception);
                throw new InvalidOperationException("Roster persistence is unavailable.", exception);
            }
        }
        finally
        {
            schemaGate.Release();
        }
    }

    private void LogPersistenceFailure(string operation, string failureCategory, Exception exception)
    {
        if (exception is MySqlException mySqlException)
        {
            logger.LogError(
                "Roster persistence operation failed. Operation={Operation} StorageMode={StorageMode} FailureCategory={FailureCategory} ExceptionType={ExceptionType} MySqlNumber={MySqlNumber} SqlState={SqlState} ServerMessage={ServerMessage}",
                operation,
                StorageMode.AzureMySqlInApp,
                failureCategory,
                exception.GetType().Name,
                mySqlException.Number,
                mySqlException.SqlState,
                mySqlException.Message);
            return;
        }

        logger.LogError(
            "Roster persistence operation failed. Operation={Operation} StorageMode={StorageMode} FailureCategory={FailureCategory} ExceptionType={ExceptionType}",
            operation,
            StorageMode.AzureMySqlInApp,
            failureCategory,
            exception.GetType().Name);
    }
}