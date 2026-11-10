using Roster26FS001.Models;
using Roster26FS001.Services;

namespace Roster26FS001.Tests.Services;

[TestClass]
public sealed class InMemoryRosterStoreTests
{
    [TestMethod]
    public void Add_PreservesSubmissionOrderAndAllowsDuplicates()
    {
        var store = new InMemoryRosterStore();
        var first = CreateEntry("Ada");
        var second = CreateEntry("Grace");

        store.Add(first);
        store.Add(second);
        store.Add(first);

        CollectionAssert.AreEqual(new[] { first, second, first }, store.GetSnapshot().ToArray());
    }

    [TestMethod]
    public void GetSnapshot_ReturnsCopyThatCannotMutateStore()
    {
        var store = new InMemoryRosterStore();
        store.Add(CreateEntry("Ada"));

        var snapshot = store.GetSnapshot();
        var copied = snapshot.ToList();
        copied.Clear();

        Assert.AreEqual(1, store.GetSnapshot().Count);
    }

    [TestMethod]
    public void Create_TrimsAllValues()
    {
        var entry = RosterEntry.Create(" Ada ", " Lovelace ", " CS ", " Ctrl+K ", " Command palette ");

        Assert.AreEqual("Ada", entry.FirstName);
        Assert.AreEqual("Lovelace", entry.LastName);
        Assert.AreEqual("CS", entry.Major);
        Assert.AreEqual("Ctrl+K", entry.FavoriteKeyboardShortcut);
        Assert.AreEqual("Command palette", entry.ShortcutUsage);
    }

    [TestMethod]
    public void Store_SupportsConcurrentAdds()
    {
        var store = new InMemoryRosterStore();

        Parallel.For(0, 100, index => store.Add(CreateEntry($"User{index}")));

        Assert.AreEqual(100, store.GetSnapshot().Count);
    }

    private static RosterEntry CreateEntry(string firstName) => RosterEntry.Create(
        firstName,
        "User",
        "Computer Science",
        "Ctrl+K",
        "Opening the command palette");
}
