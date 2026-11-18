using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roster26FS001.Models;
using Roster26FS001.Services;

namespace Roster26FS001.Pages;

public class IndexModel : PageModel
{
    private readonly IRosterStore rosterStore;

    public IndexModel(IRosterStore rosterStore)
    {
        this.rosterStore = rosterStore;
    }

    [BindProperty]
    public RosterSubmission Input { get; set; } = new();

    public IReadOnlyList<RosterEntry> Entries { get; private set; } = [];

    public bool PersistenceUnavailable { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadEntriesAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadEntriesAsync(cancellationToken);
            return Page();
        }

        try
        {
            await rosterStore.AddAsync(RosterEntry.Create(
                Input.FirstName,
                Input.LastName,
                Input.Major,
                Input.FavoriteKeyboardShortcut,
                Input.ShortcutUsage), cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadEntriesAsync(cancellationToken);
            return Page();
        }

        Input = new RosterSubmission();
        await LoadEntriesAsync(cancellationToken);
        return Page();
    }

    private async Task LoadEntriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            Entries = await rosterStore.GetAllAsync(cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            PersistenceUnavailable = true;
            ModelState.AddModelError(string.Empty, exception.Message);
            Entries = [];
        }
    }
}

public sealed class RosterSubmission
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name must be 100 characters or fewer.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name must be 100 characters or fewer.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Major is required.")]
    [StringLength(150, ErrorMessage = "Major must be 150 characters or fewer.")]
    public string Major { get; set; } = string.Empty;

    [Required(ErrorMessage = "Favorite keyboard shortcut is required.")]
    [StringLength(100, ErrorMessage = "Favorite keyboard shortcut must be 100 characters or fewer.")]
    public string FavoriteKeyboardShortcut { get; set; } = string.Empty;

    [Required(ErrorMessage = "Where the shortcut can be used is required.")]
    [StringLength(250, ErrorMessage = "Shortcut usage must be 250 characters or fewer.")]
    public string ShortcutUsage { get; set; } = string.Empty;
}
