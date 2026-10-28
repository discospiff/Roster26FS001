# RosterVideo Data Model

## RosterEntry

Represents one accepted user submission during the current application run.

| Field | Type | Required | Rules |
|---|---|---:|---|
| `FirstName` | string | Yes | Trim surrounding whitespace; non-blank; maximum 100 characters. |
| `LastName` | string | Yes | Trim surrounding whitespace; non-blank; maximum 100 characters. |
| `Major` | string | Yes | Trim surrounding whitespace; non-blank; maximum 150 characters. |
| `FavoriteKeyboardShortcut` | string | Yes | Trim surrounding whitespace; non-blank; maximum 100 characters. |
| `ShortcutUsage` | string | Yes | Trim surrounding whitespace; non-blank; maximum 250 characters. |

## RosterStore

An application-wide in-memory collection of `RosterEntry` values.

- Entries are appended in submission order.
- Duplicate values are allowed because each submission is a separate entry.
- Reads return a snapshot so callers cannot mutate the store directly.
- Data is lost when the process restarts.
- Concurrent requests must not corrupt the collection or produce partial entries.

## Validation

- All five fields are required.
- Empty or whitespace-only values are invalid.
- Maximum lengths must be enforced consistently on the form model and server-side validation path.
- Rejected submissions do not change the collection.
