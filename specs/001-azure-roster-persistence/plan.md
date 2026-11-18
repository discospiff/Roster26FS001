# Implementation Plan: Azure Roster Persistence

**Branch**: `InAppDatabase` | **Date**: 2026-11-17 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-azure-roster-persistence/spec.md`

## Summary

Add Azure-only MySQL In App persistence while preserving the existing in-memory store
for local and Visual Studio execution. Select the storage implementation outside the
Razor Page, normalize `MYSQLCONNSTR_localdb` into the MySQL provider's syntax, preserve
the generated Azure port, and never assume port 3306. Surface persistence failures
without silently losing an Azure submission to memory.

## Technical Context

**Language/Version**: C# with .NET 10

**Primary Dependencies**: ASP.NET Core Razor Pages, existing in-memory store, a maintained MySQL ADO.NET provider such as MySqlConnector, and built-in configuration, dependency injection, logging, and validation APIs

**Storage**: In-memory store locally and in Visual Studio; MySQL In App in Azure using `MYSQLCONNSTR_localdb`

**Testing**: Existing MSTest project; unit tests for normalization and mode selection, page tests for shared behavior, and opt-in MySQL integration tests for a configured database

**Target Platform**: Local Visual Studio execution and Azure App Service with MySQL In App

**Project Type**: Server-rendered Razor Pages web application

**Performance Goals**: Preserve the existing two-second Index-page target for up to 1,000 entries; use asynchronous database I/O and avoid repeated reads within one request. Validate this with a repeatable warm-path measurement that includes the page request, store read, and rendering path but excludes deployment provisioning.

**Constraints**: Azure-only database access; no local database requirement; no silent fallback after Azure persistence failure; generated port from `MYSQLCONNSTR_localdb` is authoritative; port 3306 must never be assumed; credentials must not be logged; Azure schema initialization must be idempotent and complete before the first roster operation; Azure read failures must not appear as an empty roster; concurrent writes must preserve every accepted entry and use UTC creation time plus stable identity for ordering

**Scale/Scope**: One existing Index page and JSON feed, one provider-neutral roster contract, two storage implementations, one connection-string normalizer, one idempotent MySQL table, environment selection, safe structured diagnostics, a repeatable performance measurement, and automated tests for local and Azure-specific behavior

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Code quality**: PASS — isolate provider-specific logic behind a storage contract and
  keep page handlers independent of the database provider.
- **Testing standards**: PASS — cover local mode, Azure mode selection, normalization,
  persistence failure, and shared Index-page behavior with automated tests.
- **User experience consistency**: PASS — preserve the existing form, validation, list,
  empty state, accessibility, and actionable persistence errors.
- **Performance requirements**: PASS — use async database operations, bounded reads, and
  the existing 1,000-entry/two-second target.
- **Secure delivery**: PASS — consume Azure-managed configuration, parameterize SQL, and
	redact connection secrets and provider exception details from logs and exceptions while
  retaining safe structured operation and failure-category context.
- **Technology constraints**: PASS — target .NET 10 Razor Pages and use C# platform APIs
  plus only the required MySQL provider dependency.

## Project Structure

### Documentation (this feature)

```text
specs/001-azure-roster-persistence/
├── spec.md              # Approved feature specification
├── plan.md              # This implementation plan
├── research.md          # Phase 0 decisions and constraints
├── data-model.md        # Phase 1 persistence model
├── quickstart.md        # Local, Azure, and failure verification
└── contracts/
	└── storage.md       # Storage and connection normalization contract
```

### Source Code (repository root)

```text
Roster26FS001/
├── Models/RosterEntry.cs
├── Services/
│   ├── IRosterStore.cs
│   ├── InMemoryRosterStore.cs
│   ├── AzureMySqlRosterStore.cs
│   ├── AzureInAppConnectionStringNormalizer.cs
│   └── RosterStoreSelector.cs
├── Data/
│   └── MySqlRosterSchema.cs
├── Pages/Index.cshtml(.cs)
└── Program.cs

Roster26FS001.Tests/
├── Services/
│   ├── AzureInAppConnectionStringNormalizerTests.cs
│   ├── RosterStoreSelectorTests.cs
│   └── AzureMySqlRosterStoreTests.cs
└── Pages/IndexPersistenceTests.cs
```

**Structure Decision**: Add a provider-neutral store contract and retain the current
in-memory implementation. Add a MySQL In App implementation and a dedicated normalizer
for the Azure environment variable. Select the implementation through dependency
injection and environment configuration; keep the Index PageModel dependent only on the
store contract, and make the JSON feed depend on the same active store. Initialize the
MySQL schema idempotently before the first Azure read or write, surface initialization and
read failures as persistence failures, and order records by UTC creation timestamp followed
by stable database identity. Use an opt-in integration-test fixture for a real MySQL endpoint
and a repeatable warm-path page-load measurement for up to 1,000 entries.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations identified. | The provider-neutral store is required to preserve local behavior and testability. |

## Post-Design Constitution Check

- **PASS**: The design isolates MySQL-specific code and preserves the existing Razor
  Pages user experience.
- **PASS**: Automated tests can run without Azure; real MySQL integration tests are opt-in.
- **PASS**: The generated Azure port is preserved from `MYSQLCONNSTR_localdb`; port 3306
  is never used as an implicit default.
- **PASS**: Credentials remain in environment-managed configuration, SQL is parameterized,
  and database initialization, read, and write failures are observable without exposing
  secrets or provider exception details.
