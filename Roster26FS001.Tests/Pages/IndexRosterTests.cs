using Microsoft.AspNetCore.Mvc.RazorPages;
using Roster26FS001.Models;
using Roster26FS001.Pages;
using Roster26FS001.Services;

namespace Roster26FS001.Tests.Pages;

[TestClass]
public sealed class IndexRosterTests
{
    [TestMethod]
    public void OnGet_WithNoEntries_ReturnsEmptySnapshot()
    {
        var model = new IndexModel(new InMemoryRosterStore());

        model.OnGet();

        Assert.IsEmpty(model.Entries);
    }

    [TestMethod]
    public void OnGet_ReturnsAllEntriesInSubmissionOrder()
    {
        var store = new InMemoryRosterStore();
        var first = CreateEntry("Ada");
        var second = CreateEntry("Grace");
        store.Add(first);
        store.Add(second);

        var model = new IndexModel(store);
        model.OnGet();

        CollectionAssert.AreEqual(new[] { first, second }, model.Entries.ToArray());
    }

    [TestMethod]
    public void OnPost_WithInvalidInput_LoadsExistingRoster()
    {
        var store = new InMemoryRosterStore();
        var existing = CreateEntry("Ada");
        store.Add(existing);
        var model = new IndexModel(store);
        model.ModelState.AddModelError("Input.FirstName", "First name is required.");

        var result = model.OnPost();

        Assert.IsInstanceOfType<PageResult>(result);
        CollectionAssert.AreEqual(new[] { existing }, model.Entries.ToArray());
    }

    private static RosterEntry CreateEntry(string firstName) => RosterEntry.Create(
        firstName,
        "User",
        "Computer Science",
        "Ctrl+K",
        "Opening the command palette");
}
