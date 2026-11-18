using Microsoft.AspNetCore.Mvc.RazorPages;
using Roster26FS001.Models;
using Roster26FS001.Pages;
using Roster26FS001.Services;

namespace Roster26FS001.Tests.Pages;

[TestClass]
public sealed class IndexRosterTests
{
    [TestMethod]
    public async Task OnGet_WithNoEntries_ReturnsEmptySnapshot()
    {
        var model = new IndexModel(new InMemoryRosterStore());

        await model.OnGetAsync(CancellationToken.None);

        Assert.IsEmpty(model.Entries);
    }

    [TestMethod]
    public async Task OnGet_ReturnsAllEntriesInSubmissionOrder()
    {
        var store = new InMemoryRosterStore();
        var first = CreateEntry("Ada");
        var second = CreateEntry("Grace");
        store.Add(first);
        store.Add(second);

        var model = new IndexModel(store);
        await model.OnGetAsync(CancellationToken.None);

        CollectionAssert.AreEqual(new[] { first, second }, model.Entries.ToArray());
    }

    [TestMethod]
    public async Task OnPost_WithInvalidInput_LoadsExistingRoster()
    {
        var store = new InMemoryRosterStore();
        var existing = CreateEntry("Ada");
        store.Add(existing);
        var model = new IndexModel(store);
        model.ModelState.AddModelError("Input.FirstName", "First name is required.");

        var result = await model.OnPostAsync(CancellationToken.None);

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
