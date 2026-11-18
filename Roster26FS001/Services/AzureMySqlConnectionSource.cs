namespace Roster26FS001.Services;

public sealed class AzureMySqlConnectionSource
{
    public const string DefaultConnectionFilePath = @"D:\home\data\mysql\MYSQLCONNSTR_localdb.ini";

    public string? Resolve(
        IConfiguration configuration,
        string? connectionFilePath = null,
        Func<string, bool>? fileExists = null,
        Func<string, string>? readFile = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var configuredValue = configuration.GetConnectionString("localdb")
            ?? configuration["MYSQLCONNSTR_localdb"]
            ?? Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb");

        if (!string.IsNullOrWhiteSpace(configuredValue))
        {
            return configuredValue;
        }

        var path = connectionFilePath ?? DefaultConnectionFilePath;
        fileExists ??= File.Exists;
        readFile ??= File.ReadAllText;

        if (!fileExists(path))
        {
            return null;
        }

        var fileContent = readFile(path).Trim();
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return null;
        }

        const string variableName = "MYSQLCONNSTR_localdb";
        var assignmentPrefix = variableName + "=";
        if (fileContent.StartsWith(assignmentPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return fileContent[assignmentPrefix.Length..].Trim().Trim('"');
        }

        foreach (var line in fileContent.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (line.StartsWith(assignmentPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return line[assignmentPrefix.Length..].Trim().Trim('"');
            }
        }

        return fileContent;
    }
}
