using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Roster26FS001.Models;
using Roster26FS001.Pages;
using Roster26FS001.Services;

namespace Roster26FS001.Tests.Pages;

[TestClass]
public sealed class JsonFeedPageTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [TestMethod]
    public void OnGet_WithEmptyStore_ReturnsEmptyJsonCollection()
    {
        var model = new JsonFeedModel(new InMemoryRosterStore());

        var result = model.OnGet();

        var json = JsonSerializer.Serialize(result.Value, JsonOptions);
        Assert.AreEqual("[]", json);
    }

    [TestMethod]
    public void OnGet_ReturnsAllFieldsInSubmissionOrderWithCamelCaseNames()
    {
        var store = new InMemoryRosterStore();
        store.Add(RosterEntry.Create("Ada", "Lovelace", "Computer Science", "Ctrl+K", "Command palette"));
        store.Add(RosterEntry.Create("Grace", "Hopper", "Mathematics", "Ctrl+Shift+P", "Command search"));
        var model = new JsonFeedModel(store);

        var result = model.OnGet();

        var json = JsonSerializer.Serialize(result.Value, JsonOptions);
        using var document = JsonDocument.Parse(json);
        var entries = document.RootElement.EnumerateArray().ToArray();

        Assert.AreEqual("Ada", entries[0].GetProperty("firstName").GetString());
        Assert.AreEqual("Ctrl+K", entries[0].GetProperty("favoriteKeyboardShortcut").GetString());
        Assert.AreEqual("Command palette", entries[0].GetProperty("shortcutUsage").GetString());
        Assert.AreEqual("Grace", entries[1].GetProperty("firstName").GetString());
    }

    [TestMethod]
    public void OnGet_ReturnsJsonResult()
    {
        var model = new JsonFeedModel(new InMemoryRosterStore());

        var result = model.OnGet();

        Assert.IsInstanceOfType<JsonResult>(result);
    }
}
