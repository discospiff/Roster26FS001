# Tasks: Azure Roster Persistence

**Input**: Design documents from `specs/001-azure-roster-persistence/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/storage.md`, and `quickstart.md`

**Organization**: Tasks are grouped by user story so each story can be implemented and tested as an independent increment.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the provider dependency, source structure, and test seams required by the approved plan.

- [ ] T001 Add a maintained MySQL ADO.NET provider compatible with .NET 10 to `Roster26FS001/Roster26FS001.csproj` and record the dependency rationale in the project documentation or plan notes.
- [ ] T002 [P] Create `Roster26FS001/Models/RosterEntry.cs`, `Roster26FS001/Services/IRosterStore.cs`, `Roster26FS001/Services/InMemoryRosterStore.cs`, `Roster26FS001/Services/RosterStoreSelector.cs`, and `Roster26FS001/Data/MySqlRosterSchema.cs` when the planned directories do not already exist.
- [ ] T003 [P] Create the planned test files `Roster26FS001.Tests/Services/AzureInAppConnectionStringNormalizerTests.cs`, `Roster26FS001.Tests/Services/RosterStoreSelectorTests.cs`, and `Roster26FS001.Tests/Services/AzureMySqlRosterStoreTests.cs` under the existing MSTest project.
- [ ] T004 [P] Review the existing `IRosterStore`, `InMemoryRosterStore`, `Index` PageModel, and dependency-registration code to identify the exact integration points for the new provider-neutral contract.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define shared data, storage, configuration, and database-shape behavior before implementing any user story.

**Critical**: Complete this phase before user story implementation.

- [ ] T005 Define or update `Roster26FS001/Models/RosterEntry.cs` with a stable persisted identifier, UTC creation timestamp, and the existing five fields: `FirstName` max 100 characters, `LastName` max 100 characters, `Major` max 150 characters, `FavoriteKeyboardShortcut` max 100 characters, and `ShortcutUsage` max 250 characters.
- [ ] T006 Define the provider-neutral asynchronous storage contract in `Roster26FS001/Services/IRosterStore.cs` with `AddAsync(RosterEntry entry, CancellationToken cancellationToken)` and `GetAllAsync(CancellationToken cancellationToken)`.
- [ ] T007 [P] Define idempotent schema SQL in `Roster26FS001/Data/MySqlRosterSchema.cs` with generated identity, all five roster fields, UTC creation timestamp, and indexes/order support; make repeated initialization safe and ensure existing data survives restart, redeployment, and scale-out.
- [ ] T008 [P] Define storage-mode selection types and configuration boundaries in `Roster26FS001/Services/RosterStoreSelector.cs`, ensuring configuration presence alone cannot select Azure mode during local execution.
- [ ] T009 [P] Add shared test fixtures or test helpers under `Roster26FS001.Tests/Services/` for deterministic environment selection, cancellation tokens, and store-contract assertions without requiring Azure resources.
- [ ] T010 Update `Roster26FS001/Program.cs` registration design so the Index PageModel and `Roster26FS001/Pages/JsonFeed.cshtml.cs` depend only on `IRosterStore`, and local execution remains independent of MySQL configuration.

**Checkpoint**: Shared model, contract, schema, selection boundary, and dependency-injection seam are ready for story implementation.

---

## Phase 3: User Story 1 - Persist Azure Roster Submissions (Priority: P1) MVP

**Goal**: Save accepted Azure submissions to MySQL In App and load them after restart, redeployment, or scale-out without silently falling back to memory.

**Independent Test**: In a configured Azure-like environment, submit a valid entry, verify it is accepted by the database, restart or recreate the store, and verify the entry is returned in submission order; make the database unavailable and verify the submission is not reported as saved.

### Tests for User Story 1

> Write these tests first and confirm they fail before implementation.

- [ ] T011 [P] [US1] Add normalization tests in `Roster26FS001.Tests/Services/AzureInAppConnectionStringNormalizerTests.cs` for valid Azure key/value input, canonical provider-key mapping, exact generated-port preservation, missing-port rejection, invalid-port rejection, incomplete-value rejection, and password redaction from exception text.
- [ ] T012 [P] [US1] Add Azure store tests in `Roster26FS001.Tests/Services/AzureMySqlRosterStoreTests.cs` for parameterized add/read behavior, idempotent schema initialization, submission ordering by UTC creation timestamp plus stable identity, duplicate preservation, successful-write-before-success semantics, surfaced read/write failures, and non-empty failure handling; keep real MySQL cases opt-in.
- [ ] T013 [P] [US1] Add selection tests in `Roster26FS001.Tests/Services/RosterStoreSelectorTests.cs` for Azure hosting selection, local hosting selection, Azure-looking configuration outside Azure, and missing or malformed Azure configuration.
- [ ] T014 [P] [US1] Add persistence page tests in `Roster26FS001.Tests/Pages/IndexPersistenceTests.cs` for successful Azure submission, schema-initialization failure, read failure without a misleading empty state, retained values after persistence failure, no success message after failure, and shared Index-page integration.

### Implementation for User Story 1

- [ ] T015 [US1] Implement `Roster26FS001/Services/AzureInAppConnectionStringNormalizer.cs` to parse `MYSQLCONNSTR_localdb`, map Azure key names to the selected provider's server, port, database, user, and password keys, preserve the exact generated port, reject missing or invalid ports, and never inject port 3306.
- [ ] T016 [US1] Implement `Roster26FS001/Services/AzureMySqlRosterStore.cs` using asynchronous provider APIs, parameterized SQL, idempotent schema initialization before the first read/write, stable UTC-timestamp-plus-identity ordering, duplicate preservation, safe failure-category logging, and failure propagation without in-memory fallback or empty-read substitution.
- [ ] T017 [US1] Implement `Roster26FS001/Services/RosterStoreSelector.cs` so only the active Azure hosting environment selects `AzureMySqlRosterStore`; local and Visual Studio execution always selects `InMemoryRosterStore` and never opens a MySQL connection.
- [ ] T018 [US1] Register the Azure store, normalizer, selector, and schema services in `Roster26FS001/Program.cs` using environment-appropriate configuration and without logging credentials or raw connection strings.
- [ ] T019 [US1] Update `Roster26FS001/Pages/Index.cshtml.cs` to await the provider-neutral store for reads and writes, report success only after Azure acceptance, distinguish read failure from an empty roster, surface actionable persistence errors, and preserve entered values for retry.
- [ ] T020 [US1] Add opt-in real-MySQL integration coverage in `Roster26FS001.Tests/Services/AzureMySqlRosterStoreTests.cs` for repeated schema initialization, add/read persistence, restart continuity, generated-port use, concurrent submissions, stable ordering under equal timestamps, and database failure behavior when a configured test connection is explicitly enabled.

**Checkpoint**: User Story 1 is independently functional and testable in Azure-like and opt-in real-MySQL environments.

---

## Phase 4: User Story 2 - Preserve Local In-Memory Behavior (Priority: P1)

**Goal**: Keep local and Visual Studio execution fast, database-free, and application-lifetime in memory.

**Independent Test**: Run the application without Azure hosting or Azure database access, submit a valid entry, confirm it appears during the run, confirm no MySQL connection is attempted, restart the application, and confirm the entry is gone.

### Tests for User Story 2

- [ ] T021 [P] [US2] Extend `Roster26FS001.Tests/Services/RosterStoreSelectorTests.cs` to prove local and Visual Studio environments select `InMemoryRosterStore` even when `MYSQLCONNSTR_localdb` is present.
- [ ] T022 [P] [US2] Add or extend in-memory store tests under `Roster26FS001.Tests/Services/` to verify accepted entries are available during the application lifetime, duplicates are preserved, ordering is deterministic, and no database dependency is required.
- [ ] T023 [P] [US2] Extend `Roster26FS001.Tests/Pages/IndexPersistenceTests.cs` to verify local submissions use the existing form flow, local reads remain available during the run, local restart semantics are documented/testable, local mode never constructs or opens a MySQL connection, and validation behavior is unchanged.

### Implementation for User Story 2

- [ ] T024 [US2] Preserve or update `Roster26FS001/Services/InMemoryRosterStore.cs` to implement the shared asynchronous contract, retain application-lifetime behavior, preserve duplicate entries and submission order, and avoid any MySQL calls.
- [ ] T025 [US2] Complete local dependency-injection configuration in `Roster26FS001/Program.cs` so local and Visual Studio startup does not require `MYSQLCONNSTR_localdb`, MySQL schema initialization, or Azure resources.
- [ ] T026 [US2] Verify and adjust `Roster26FS001/Pages/Index.cshtml.cs` only as needed so local storage failures and validation continue to use existing user-facing behavior without Azure-specific branching in the page.

**Checkpoint**: User Stories 1 and 2 both work independently, with local execution remaining database-free.

---

## Phase 5: User Story 3 - Keep Roster Behavior Consistent Across Storage Modes (Priority: P2)

**Goal**: Preserve the same Index-page fields, validation, ordering, accessibility, terminology, and JSON feed behavior across storage modes.

**Independent Test**: Submit equivalent valid and invalid values through local and Azure-backed stores and compare displayed fields, validation outcomes, ordering, error behavior, and active roster feed output.

### Tests for User Story 3

- [ ] T027 [P] [US3] Extend `Roster26FS001.Tests/Pages/IndexPersistenceTests.cs` to compare valid and invalid submission behavior, all five displayed fields, ordering, empty-state behavior, persistence errors, labels, validation messages, and keyboard-accessible form behavior across storage modes.
- [ ] T028 [P] [US3] Update `Roster26FS001.Tests/Pages/JsonFeedPageTests.cs` to verify `Roster26FS001/Pages/JsonFeed.cshtml.cs` exposes the active roster collection consistently for both in-memory and Azure-backed stores.
- [ ] T029 [P] [US3] Add contract-level tests under `Roster26FS001.Tests/Services/` proving both store implementations return equivalent field values, preserve duplicates, and use the documented submission-order rule.

### Implementation for User Story 3

- [ ] T030 [US3] Update `Roster26FS001/Pages/Index.cshtml.cs` and `Roster26FS001/Pages/Index.cshtml` only where necessary to preserve existing validation, accessible labels, validation summaries, error messaging, keyboard access, and user-facing terminology for both storage modes.
- [ ] T031 [US3] Update `Roster26FS001/Pages/JsonFeed.cshtml.cs` and `Roster26FS001/Program.cs` so the JSON feed reads from the active `IRosterStore`, shares the Index-page collection, and does not create a separate storage path.
- [ ] T032 [US3] Update `specs/001-azure-roster-persistence/contracts/storage.md` with the deterministic UTC-timestamp-plus-identity ordering rule, duplicate-entry behavior, empty-roster behavior, read-failure behavior, and storage-mode differences without exposing provider secrets.

**Checkpoint**: All user stories are independently functional and present the same roster contract to users.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate the complete feature against the constitution, quickstart, performance, security, and delivery requirements.

- [ ] T033 [P] Update `specs/001-azure-roster-persistence/quickstart.md` with the final environment variable names, Azure setup assumptions, opt-in integration-test instructions, idempotent schema-initialization behavior, read-failure behavior, and warm-path performance measurement procedure.
- [ ] T034 [P] Review logging and exception handling in `Roster26FS001/Services/AzureInAppConnectionStringNormalizer.cs`, `Roster26FS001/Services/AzureMySqlRosterStore.cs`, and `Roster26FS001/Pages/Index.cshtml.cs` to ensure diagnostics contain only operation, storage mode, and failure category, while passwords, raw connection strings, and secret-bearing provider exception text never appear in logs, responses, diagnostics, or test output.
- [ ] T035 [P] Review MySQL queries and connection usage in `Roster26FS001/Services/AzureMySqlRosterStore.cs` for asynchronous I/O, bounded reads, appropriate indexing/order support, stable ordering, and no unnecessary repeated database calls within one request.
- [ ] T036 Run the existing automated test suite and the non-Azure feature tests from `Roster26FS001.Tests/Roster26FS001.Tests.csproj` without Azure resources; resolve failures before completion.
- [ ] T037 Run the opt-in MySQL integration tests only with an explicitly configured test database and verify generated-port preservation, restart continuity, failure handling, and secret redaction.
- [ ] T038 Build `Roster26FS001.slnx` in Release with warnings treated as errors, then review the resulting test and build output against the constitution's quality gates.
- [ ] T039 Execute the local, Azure configuration, and failure verification steps in `specs/001-azure-roster-persistence/quickstart.md`, including a repeatable warm-path measurement that loads and renders up to 1,000 entries within 2 seconds in both storage modes, excluding deployment provisioning.
- [ ] T040 Review `specs/001-azure-roster-persistence/checklists/persistence-quality.md` and record any unresolved requirements-quality findings before implementation is considered complete.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies; can start immediately.
- **Phase 2 Foundational**: Depends on Phase 1 and blocks all user stories.
- **Phase 3 User Story 1**: Depends on Phase 2; delivers the MVP Azure persistence path.
- **Phase 4 User Story 2**: Depends on Phase 2 and the shared contract; can proceed in parallel with US1 after foundational work, but final integration depends on the selector and DI seam.
- **Phase 5 User Story 3**: Depends on the shared contract and the completed store implementations from US1 and US2.
- **Phase 6 Polish**: Depends on all selected user stories being implemented and tested.

### User Story Dependencies

- **US1 (P1)**: Depends on Foundational; no dependency on US2 behavior beyond the shared `IRosterStore` contract.
- **US2 (P1)**: Depends on Foundational; can be implemented in parallel with US1 when files are isolated.
- **US3 (P2)**: Depends on US1 and US2 because it compares their shared behavior and integrates the active store with the page/feed.

### Parallel Opportunities

- Phase 1 tasks T002–T004 can run in parallel after the project dependency decision in T001 is known.
- Phase 2 tasks T007–T009 can run in parallel; T005 and T006 establish shared model/contract context.
- US1 tests T011–T014 can run in parallel before implementation; T015 and T016 can proceed in parallel after the contract/schema decisions are fixed.
- US2 tests T021–T023 can run in parallel; T024 and T025 can proceed in parallel when the selector contract is stable.
- US3 tests T027–T029 can run in parallel before implementation; T030–T032 should be coordinated because they touch shared page/feed behavior.
- Polish tasks T033–T035 can run in parallel; T036–T039 are sequential validation gates as appropriate.

## Parallel Example: User Story 1

```text
Task T011: Normalize MYSQLCONNSTR_localdb and generated-port unit tests in Roster26FS001.Tests/Services/AzureInAppConnectionStringNormalizerTests.cs
Task T012: Azure store contract and failure tests in Roster26FS001.Tests/Services/AzureMySqlRosterStoreTests.cs
Task T013: Storage-mode selection tests in Roster26FS001.Tests/Services/RosterStoreSelectorTests.cs
Task T014: Index persistence page tests in Roster26FS001.Tests/Pages/IndexPersistenceTests.cs
```

## Implementation Strategy

### MVP First (User Story 1)

1. Complete Phase 1 Setup and Phase 2 Foundational.
2. Implement and test US1 Azure selection, connection normalization, schema, add/read persistence, and failure handling.
3. Run US1 tests with a configured Azure-like environment and optional real-MySQL integration tests.
4. Stop and validate the Azure persistence MVP before adding consistency refinements.

### Incremental Delivery

1. Complete Setup and Foundational phases.
2. Add US1 and validate Azure persistence independently.
3. Add US2 and validate that local behavior remains database-free.
4. Add US3 and validate shared page, validation, ordering, accessibility, and feed behavior.
5. Complete cross-cutting security, performance, quickstart, build, and test gates.

### Notes

- Every task is unchecked and must be completed by implementation review.
- `[P]` marks tasks that can proceed in parallel without depending on incomplete work in another file.
- Story labels map to the three user stories in `spec.md`.
- Real MySQL integration tests remain opt-in; ordinary local builds and tests must not require Azure resources.
- No task authorizes implementation before the user invokes `speckit.implement`.
