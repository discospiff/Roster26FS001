# RosterVideo Implementation Tasks

**Input**: Design documents from `.specify/`
**Specification**: `.specify/feature-spec.md`
**Plan**: `.specify/plan.md`

## Phase 1: Setup

**Purpose**: Establish the test project and solution structure needed for implementation.

- [ ] T001 Create the `Roster26FS001.Tests` MSTest project targeting `net10.0` in `Roster26FS001.Tests/Roster26FS001.Tests.csproj`
- [ ] T002 Add a project reference from `Roster26FS001.Tests/Roster26FS001.Tests.csproj` to `Roster26FS001/Roster26FS001.csproj` and add the test project to `Roster26FS001.slnx`
- [ ] T003 [P] Configure the MSTest SDK, test adapter, and test framework packages in `Roster26FS001.Tests/Roster26FS001.Tests.csproj`

## Phase 2: Foundational

**Purpose**: Create the shared roster model, validation rules, and application-wide in-memory storage required by every user story.

- [ ] T004 [P] Create the `RosterEntry` model with `FirstName`, `LastName`, `Major`, `FavoriteKeyboardShortcut`, and `ShortcutUsage` in `Roster26FS001/Models/RosterEntry.cs`; trim surrounding whitespace, require non-blank values, and enforce maximum lengths of 100, 100, 150, 100, and 250 characters respectively
- [ ] T005 Create the `IRosterStore` contract for adding entries and returning an immutable or copied submission-order snapshot in `Roster26FS001/Services/IRosterStore.cs`
- [ ] T006 Implement a thread-safe application-lifetime in-memory `InMemoryRosterStore` in `Roster26FS001/Services/InMemoryRosterStore.cs`; allow duplicate entries, prevent partial additions, and discard all data on process restart
- [ ] T007 Register `IRosterStore` with the application lifetime in `Roster26FS001/Program.cs` without adding database or Azure storage dependencies
- [ ] T008 [P] Write unit tests for required fields, maximum lengths, trimming, duplicate entries, submission order, snapshot isolation, and concurrent store access in `Roster26FS001.Tests/Services/InMemoryRosterStoreTests.cs`

## Phase 3: User Story 1 - Submit a roster entry (Priority: P1)

**Goal**: A user can submit all five required values and receive field-level validation when input is invalid.

**Independent Test**: Submit valid values on the home page and verify that the entry is accepted; submit blank, whitespace-only, and over-length values and verify that no invalid entry is stored.

- [ ] T009 [US1] Write Razor Pages handler tests for valid submission, invalid submission, preserved input, and no store mutation on validation failure in `Roster26FS001.Tests/Pages/IndexSubmissionTests.cs`
- [ ] T010 [US1] Add the bound submission model and `OnPost` handler to `Roster26FS001/Pages/Index.cshtml.cs`; apply server-side validation, trim accepted values, add only valid entries to `IRosterStore`, and reload the roster snapshot
- [ ] T011 [US1] Add the five labeled form controls, validation messages, validation summary, submit action, and preserved invalid values to `Roster26FS001/Pages/Index.cshtml`; use native HTML controls and associate errors with fields through generated IDs and targeted ARIA attributes
- [ ] T012 [US1] Verify the home-page form rejects blank, whitespace-only, and over-length values using the limits from `.specify/data-model.md` and displays actionable messages without adding a partial entry

## Phase 4: User Story 2 - View the roster on the home page (Priority: P1)

**Goal**: Users can see every accepted roster entry and receive a clear empty-state message when none exist.

**Independent Test**: Start with an empty store and verify the empty state, then add multiple entries and verify that all five values for every entry appear in submission order.

- [ ] T013 [US2] Write Razor Pages tests for the empty state, complete entry rendering, multiple entries, duplicate entries, and submission-order display in `Roster26FS001.Tests/Pages/IndexRosterTests.cs`
- [ ] T014 [US2] Load a submission-order roster snapshot in `Roster26FS001/Pages/Index.cshtml.cs` for every GET and invalid POST response without exposing mutable store state
- [ ] T015 [US2] Render the roster list and empty-state message in `Roster26FS001/Pages/Index.cshtml`; display first name, last name, major, favorite keyboard shortcut, and shortcut usage for every entry using encoded Razor output
- [ ] T016 [US2] Add a descriptive JSON feed navigation link to `Roster26FS001/Pages/Shared/_Layout.cshtml` while preserving the existing navigation structure

## Phase 5: User Story 3 - Retrieve the roster as JSON (Priority: P2)

**Goal**: Users and consuming clients can retrieve the same roster information as valid JSON from a separate page.

**Independent Test**: Populate the store, retrieve `/JsonFeed`, and verify that the response contains the same five values in the same order; repeat with an empty store and verify `[]`.

- [ ] T017 [US3] Write JSON feed contract tests for empty and populated responses, lower-camel-case property names, submission order, duplicate entries, and all five roster fields in `Roster26FS001.Tests/Pages/JsonFeedPageTests.cs`
- [ ] T018 [US3] Create the separate JSON feed Razor Page at `Roster26FS001/Pages/JsonFeed.cshtml.cs` with a direct GET handler that reads a store snapshot and returns the contract defined in `.specify/contracts/json-feed.md`
- [ ] T019 [US3] Add the JSON feed page view at `Roster26FS001/Pages/JsonFeed.cshtml` with the `/JsonFeed` route and a minimal accessible document context for direct navigation while preserving the JSON response contract
- [ ] T020 [US3] Verify the home-page JSON feed link navigates to `/JsonFeed` and that the feed returns `[]` when the store is empty in `Roster26FS001.Tests/Pages/JsonFeedPageTests.cs`

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Complete accessibility, responsive presentation, documentation, and quality gates across the feature.

- [ ] T021 [P] Add responsive roster and form styling, visible keyboard focus, sufficient contrast, and readable validation/status states in `Roster26FS001/wwwroot/css/site.css`
- [ ] T022 [P] Keep `Roster26FS001/wwwroot/js/site.js` limited to progressive enhancement and confirm core form submission, validation, navigation, and feed access work without JavaScript
- [ ] T023 [P] Add or update page-level accessibility markup in `Roster26FS001/Pages/Index.cshtml`, `Roster26FS001/Pages/JsonFeed.cshtml`, and `Roster26FS001/Pages/Shared/_Layout.cshtml`; verify heading hierarchy, labels, field/error associations, meaningful link text, keyboard access, and only necessary valid ARIA attributes against basic WCAG 2.2 AA checks
- [ ] T024 Update `.specify/quickstart.md` if implementation paths or local/Azure verification steps change
- [ ] T025 Run the full automated test suite and build the solution; resolve failures or warnings before completion
- [ ] T026 Perform the local Visual Studio 2026 and Azure App Service smoke checks documented in `.specify/quickstart.md`, including the intentional loss of in-memory data after restart

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies; can begin immediately.
- **Phase 2 Foundational**: Depends on Phase 1; blocks all user-story work.
- **Phase 3 User Story 1**: Depends on Phase 2 and delivers the MVP submission flow.
- **Phase 4 User Story 2**: Depends on Phase 2 and integrates with the submission flow from Phase 3 for the complete home-page experience.
- **Phase 5 User Story 3**: Depends on Phase 2 and the shared store; can be developed after the store contract exists and should be validated against entries created by Phase 3.
- **Phase 6 Polish**: Depends on the desired user stories being implemented.

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2; no dependency on US2 or US3.
- **US2 (P1)**: Can start after Phase 2; the full end-to-end home-page scenario uses the submission behavior from US1.
- **US3 (P2)**: Can start after Phase 2; uses the same store contract as US1 and US2.

### Within Each User Story

- Tests are written before implementation and must fail for the missing behavior.
- Models and contracts precede services and page handlers.
- Services precede page integration.
- A story is independently validated before moving to the next priority.

## Parallel Execution Examples

### Setup and Foundational Work

- T003 can run in parallel with T004 after T001 and T002 establish the test project.
- T004 and T008 can be prepared in parallel, but T008 execution depends on T004 and T006.
- T005 can be reviewed in parallel with T004 before T006 implements the contract.

### User Story 1

- T009 can be written while T010 and T011 are being designed, but must run before declaring the story complete.
- T010 and T011 touch different files and can be implemented in parallel after the submission model is agreed.

### User Story 2

- T013 can be written in parallel with T014 and T015.
- T016 can be implemented independently of the roster rendering changes after the feed route is established in the plan.

### User Story 3

- T017 can be written in parallel with T018.
- T019 and T020 depend on the feed route and contract but can be split between page markup and verification.

### Polish

- T021, T022, and T023 touch separate presentation assets and can run in parallel after the page structure stabilizes.

## Implementation Strategy

### MVP First

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational storage, validation, and tests.
3. Complete Phase 3 User Story 1.
4. Validate that a user can submit a valid roster entry and that invalid input is rejected without mutation.
5. Add Phase 4 User Story 2 for the complete usable roster homepage.

### Incremental Delivery

1. Add the submission flow and validate it independently.
2. Add the roster list and empty state.
3. Add the JSON feed and contract tests.
4. Complete accessibility, responsive styling, local verification, and Azure smoke checks.
5. Run the full build and test quality gate before implementation is considered complete.

## Notes

- `[P]` tasks touch different files and have no dependency on incomplete work.
- `[US1]`, `[US2]`, and `[US3]` map tasks to the corresponding feature-spec user stories.
- In-memory storage is intentional; no database, file persistence, or Azure storage should be introduced.
- Do not issue `speckit.implement` until the task list and design artifacts are approved.
