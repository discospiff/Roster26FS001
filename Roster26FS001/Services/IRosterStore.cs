using Roster26FS001.Models;

namespace Roster26FS001.Services;

public interface IRosterStore
{
    void Add(RosterEntry entry);

    IReadOnlyList<RosterEntry> GetSnapshot();
}
