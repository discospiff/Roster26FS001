using Roster26FS001.Models;

namespace Roster26FS001.Services;

public sealed class InMemoryRosterStore : IRosterStore
{
    private readonly object syncRoot = new();
    private readonly List<RosterEntry> entries = [];

    public void Add(RosterEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (syncRoot)
        {
            entries.Add(entry);
        }
    }

    public IReadOnlyList<RosterEntry> GetSnapshot()
    {
        lock (syncRoot)
        {
            return entries.ToArray();
        }
    }
}
