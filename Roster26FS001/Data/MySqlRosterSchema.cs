namespace Roster26FS001.Data;

public static class MySqlRosterSchema
{
    public const string CreateTable = """
        CREATE TABLE IF NOT EXISTS RosterEntries (
            Id BIGINT NOT NULL AUTO_INCREMENT,
            FirstName VARCHAR(100) NOT NULL,
            LastName VARCHAR(100) NOT NULL,
            Major VARCHAR(150) NOT NULL,
            FavoriteKeyboardShortcut VARCHAR(100) NOT NULL,
            ShortcutUsage VARCHAR(250) NOT NULL,
            CreatedAtUtc DATETIME(6) NOT NULL,
            PRIMARY KEY (Id),
            INDEX IX_RosterEntries_CreatedAtUtc_Id (CreatedAtUtc, Id)
        );
        """;
}