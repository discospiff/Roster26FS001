using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roster26FS001.Pages;
using Roster26FS001.Services;

namespace Roster26FS001.Tests.Pages;

[TestClass]
public sealed class IndexSubmissionTests
{
    [TestMethod]
    public void OnPost_WithValidInput_AddsEntryAndClearsForm()
    {
        var store = new InMemoryRosterStore();
        var model = new IndexModel(store)
        {
            Input = new RosterSubmission
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Major = "Computer Science",
                FavoriteKeyboardShortcut = "Ctrl+K",
                ShortcutUsage = "Opening the command palette"
            }
        };

        var result = model.OnPost();

        Assert.IsInstanceOfType<PageResult>(result);
        var entry = store.GetSnapshot()[0];
        Assert.AreEqual("Ada", entry.FirstName);
        Assert.AreEqual("Opening the command palette", entry.ShortcutUsage);
        Assert.IsTrue(string.IsNullOrEmpty(model.Input.FirstName));
    }

    [TestMethod]
    public void OnPost_WithInvalidModelState_DoesNotAddEntryAndKeepsInput()
    {
        var store = new InMemoryRosterStore();
        var model = new IndexModel(store)
        {
            Input = new RosterSubmission
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Major = "Computer Science",
                FavoriteKeyboardShortcut = "Ctrl+K",
                ShortcutUsage = "Opening the command palette"
            }
        };
        model.ModelState.AddModelError("Input.FirstName", "First name is required.");

        var result = model.OnPost();

        Assert.IsInstanceOfType<PageResult>(result);
        Assert.IsTrue(model.ModelState.ErrorCount > 0);
        Assert.AreEqual(0, store.GetSnapshot().Count);
        Assert.AreEqual("Ada", model.Input.FirstName);
    }

    [TestMethod]
    public void InputValidation_RejectsOverLengthValues()
    {
        var input = new RosterSubmission
        {
            FirstName = new string('A', 101),
            LastName = "Lovelace",
            Major = "Computer Science",
            FavoriteKeyboardShortcut = "Ctrl+K",
            ShortcutUsage = "Opening the command palette"
        };
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            input,
            new ValidationContext(input),
            results,
            validateAllProperties: true);

        Assert.IsFalse(isValid);
        StringAssert.Contains(results[0].ErrorMessage!, "100 characters");
    }
}
