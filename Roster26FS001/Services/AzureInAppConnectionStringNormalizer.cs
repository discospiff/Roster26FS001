using System.Data.Common;

namespace Roster26FS001.Services;

public sealed record AzureInAppConnection(
    string Host,
    uint Port,
    string Database,
    string UserId,
    string Password)
{
    public string ToConnectionString() =>
        new DbConnectionStringBuilder
        {
            ["Server"] = Host,
            ["Port"] = Port,
            ["Database"] = Database,
            ["User ID"] = UserId,
            ["Password"] = Password
        }.ConnectionString;
}

public sealed class AzureInAppConnectionStringNormalizer
{
    public AzureInAppConnection Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Azure MySQL configuration is missing.");
        }

        var source = new DbConnectionStringBuilder { ConnectionString = value };
        var host = GetRequired(source, "Server", "Host", "Data Source");
        var portText = GetRequired(source, "Port");
        var database = GetRequired(source, "Database", "Initial Catalog");
        var userId = GetRequired(source, "User ID", "Uid", "User");
        var password = GetRequired(source, "Password", "Pwd");

        if (!uint.TryParse(portText, out var port) || port is 0 or > 65535)
        {
            throw new InvalidOperationException("Azure MySQL configuration contains an invalid port.");
        }

        return new AzureInAppConnection(host, port, database, userId, password);
    }

    private static string GetRequired(DbConnectionStringBuilder source, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (source.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return value.ToString()!;
            }
        }

        throw new InvalidOperationException("Azure MySQL configuration is incomplete.");
    }
}