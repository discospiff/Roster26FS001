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

    public void OnGet()
    {
        string brand = "A roster of the most awesomest students ever!";
        String inBrand = Request.Query["Brand"];
        if (!string.IsNullOrWhiteSpace(inBrand))
        {
            brand = inBrand;
        }
        ViewData["Brand"] = brand; 

        LoadEntries();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadEntries();
            return Page();
        }

        rosterStore.Add(RosterEntry.Create(
            Input.FirstName,
            Input.LastName,
            Input.Major,
            Input.FavoriteKeyboardShortcut,
            Input.ShortcutUsage));

        Input = new RosterSubmission();
        LoadEntries();
        return Page();
    }

    private void LoadEntries()
    {
        Entries = rosterStore.GetSnapshot();
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
