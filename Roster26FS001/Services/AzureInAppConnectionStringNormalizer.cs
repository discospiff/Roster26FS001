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
    public AzureInAppConnection Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Azure MySQL configuration is missing.");
        }

        var source = new DbConnectionStringBuilder { ConnectionString = value };
        var hostValue = GetRequired("host", source, "Server", "Host", "Data Source", "Address", "Addr", "Network Address", "HOST");
        var host = hostValue;
        var portText = GetOptional(source, "Port", "PORT");

        if (string.IsNullOrWhiteSpace(portText) && TrySplitHostAndPort(hostValue, out var hostWithoutPort, out var portFromHost))
        {
            host = hostWithoutPort;
            portText = portFromHost;
        }

        if (string.IsNullOrWhiteSpace(portText))
        {
            throw new InvalidOperationException("Azure MySQL configuration is missing the required port value.");
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

    private static bool TrySplitHostAndPort(string value, out string host, out string port)
    {
        host = value;
        port = string.Empty;

        var separator = value.LastIndexOf(':');
        if (separator <= 0 || separator == value.Length - 1 || value[..separator].Contains(':'))
        {
            return false;
        }

        var candidatePort = value[(separator + 1)..];
        if (!uint.TryParse(candidatePort, out var parsedPort) || parsedPort is 0 or > 65535)
        {
            return false;
        }

        host = value[..separator];
        port = parsedPort.ToString();
        return true;
    }
}