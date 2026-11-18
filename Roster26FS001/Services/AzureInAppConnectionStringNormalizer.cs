using System.Data.Common;

namespace Roster26FS001.Services;

public sealed record AzureInAppConnection(
    string Host,
    uint Port,
    string Database,
    string UserId,
    string Password)
{
    public string SafeEndpoint => $"{Host}:{Port}";

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
    public AzureInAppConnection Normalize(string? value, string? portOverride = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Azure MySQL configuration is missing.");
        }

        var source = new DbConnectionStringBuilder { ConnectionString = value };
        var host = GetRequired("host", source, "Server", "Host", "Data Source", "Address", "Addr", "Network Address", "HOST");
        var portText = GetOptional(source, "Port", "PORT") ?? portOverride;
        if (string.IsNullOrWhiteSpace(portText))
        {
            throw new InvalidOperationException("Azure MySQL configuration is missing the required port value. Set Port in the connection string or configure MYSQL_PORT.");
        }
        var database = GetRequired("database", source, "Database", "Initial Catalog", "Catalog", "Db", "DBNAME");
        var userId = GetRequired("user", source, "User ID", "Uid", "User", "Username", "UserName", "UID");
        var password = GetRequired("password", source, "Password", "Pwd", "Pass", "PASSWORD");

        if (!uint.TryParse(portText, out var port) || port is 0 or > 65535)
        {
            throw new InvalidOperationException("Azure MySQL configuration contains an invalid port.");
        }

        return new AzureInAppConnection(host, port, database, userId, password);
    }

    private static string GetRequired(string fieldName, DbConnectionStringBuilder source, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (source.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return value.ToString()!;
            }
        }

        throw new InvalidOperationException($"Azure MySQL configuration is missing the required {fieldName} value.");
    }

    private static string? GetOptional(DbConnectionStringBuilder source, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (source.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return value.ToString();
            }
        }

        return null;
    }
}