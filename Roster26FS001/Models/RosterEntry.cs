using System.ComponentModel.DataAnnotations;

namespace Roster26FS001.Models;

public sealed record RosterEntry
{
    public long Id { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    [Required]
    [StringLength(100)]
    public required string FirstName { get; init; }

    [Required]
    [StringLength(100)]
    public required string LastName { get; init; }

    [Required]
    [StringLength(150)]
    public required string Major { get; init; }

    [Required]
    [StringLength(100)]
    public required string FavoriteKeyboardShortcut { get; init; }

    [Required]
    [StringLength(250)]
    public required string ShortcutUsage { get; init; }

    public static RosterEntry Create(
        string firstName,
        string lastName,
        string major,
        string favoriteKeyboardShortcut,
        string shortcutUsage)
    {
        return new RosterEntry
        {
            CreatedAtUtc = DateTime.UtcNow,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Major = major.Trim(),
            FavoriteKeyboardShortcut = favoriteKeyboardShortcut.Trim(),
            ShortcutUsage = shortcutUsage.Trim()
        };
    }
}
