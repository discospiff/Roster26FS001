# Azure Roster Persistence Data Model

## RosterEntry

Represents one accepted roster submission.

| Field | Type | Required | Rules |
|---|---|---:|---|
| `Id` | generated identifier | Yes for persisted records | Stable identifier for database identity; must not change when an entry is read. |
| `FirstName` | string | Yes | Preserve the existing validation and maximum length of 100 characters. |
| `LastName` | string | Yes | Preserve the existing validation and maximum length of 100 characters. |
| `Major` | string | Yes | Preserve the existing validation and maximum length of 150 characters. |
| `FavoriteKeyboardShortcut` | string | Yes | Preserve the existing validation and maximum length of 100 characters. |
| `ShortcutUsage` | string | Yes | Preserve the existing validation and maximum length of 250 characters. |
| `CreatedAt` | UTC timestamp | Yes for persisted records | Set when the entry is accepted for storage; used to preserve submission order. |

## StorageMode

Represents the active persistence behavior.

- `InMemory`: used for local execution and Visual Studio.
- `AzureMySqlInApp`: used only when the application is running in the Azure deployment environment.
- Storage selection must be deterministic and testable.
- Configuration presence alone must not select Azure mode during local execution.

## AzureInAppConnection

Represents the normalized connection details derived from `MYSQLCONNSTR_localdb`.

- `Server` or host is read from the Azure value.
- `Port` is read from the Azure value and is required; it must not default to 3306.
- `Database`, `UserId`, and `Password` are read from the Azure value.
- The original secret value must not be written to logs or source control.
- Missing, malformed, or incomplete values produce a clear configuration failure before database operations begin.

## RosterStore Contract

Both storage implementations expose equivalent operations:

- Add one validated entry and report success only after the selected store accepts it.
- Return all entries in documented submission order.
- Preserve duplicate submissions.
- Surface read/write failures to the page flow rather than silently switching stores.
