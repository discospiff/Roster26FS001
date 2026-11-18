# Feature Specification: Azure Roster Persistence

**Feature Branch**: `InAppDatabase`

**Created**: 2026-11-17

**Status**: Ready for implementation planning

**Input**: User description: "I have created a new branch, InAppDatabase, for this existing Roster Application. When a user enters a new entry on the Index page and clicks Submit, I would like to persist this to a database, but only when the application is running in Azure. When the application runs locally, or in Visual Studio, it should still use the in memory roster that already exists."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Persist Azure roster submissions (Priority: P1)

As a user of the Azure-hosted roster application, I want a submitted roster entry to be saved in the application's database so that it remains available after the application restarts or is redeployed.

**Why this priority**: Azure persistence is the primary purpose of this feature and protects roster data in the deployed environment.

**Independent Test**: In an Azure deployment configured for this feature, submit a valid roster entry, restart or redeploy the application, and verify that the entry is still shown on the Index page.

**Acceptance Scenarios**:

1. **Given** the application is running in Azure and the database is available, **When** a user submits a valid roster entry, **Then** the entry is saved and appears in the roster.
2. **Given** the application is running in Azure and contains saved entries, **When** the application restarts, **Then** the saved entries remain available on the Index page.
3. **Given** the application is running in Azure and the database is unavailable, **When** a user submits a roster entry, **Then** the application does not report the entry as saved and displays an actionable error without silently storing it only in memory.
4. **Given** the application is running in Azure and the database schema is missing, **When** the application initializes persistence, **Then** it creates the required schema idempotently before serving roster operations, or reports a safe persistence-unavailable error without accepting submissions if initialization fails.
5. **Given** multiple Azure instances submit entries concurrently, **When** the roster is loaded, **Then** every accepted entry appears exactly once, duplicates remain allowed, and entries are ordered by the persisted UTC creation sequence with a stable identity tie-breaker.

---

### User Story 2 - Preserve local in-memory behavior (Priority: P1)

As a developer running the application locally or in Visual Studio, I want the existing in-memory roster behavior to remain unchanged so that local development does not require an Azure database.

**Why this priority**: Local development must remain fast, simple, and independent of Azure resources.

**Independent Test**: Run the application locally and in Visual Studio without Azure database configuration, submit a valid entry, and verify that it appears during the current run without requiring a database connection.

**Acceptance Scenarios**:

1. **Given** the application is running locally or in Visual Studio without Azure database configuration, **When** a user submits a valid roster entry, **Then** the entry is stored in memory and appears in the roster.
2. **Given** the application is running locally or in Visual Studio, **When** the application restarts, **Then** locally submitted entries are no longer available.
3. **Given** the application is running locally or in Visual Studio, **When** the Index page is used, **Then** the application does not attempt to connect to the Azure database.

---

### User Story 3 - Keep roster behavior consistent across storage modes (Priority: P2)

As a user, I want the Index page to display and validate roster entries consistently regardless of whether the application uses Azure persistence or local in-memory storage.

**Why this priority**: Users should not need to understand the deployment environment to use the roster.

**Independent Test**: Exercise the same valid and invalid submissions in Azure and local environments and compare validation, display, ordering, and error behavior.

**Acceptance Scenarios**:

1. **Given** the same valid roster values, **When** the values are submitted in either storage mode, **Then** the entry is displayed with the same five roster fields.
2. **Given** invalid roster values, **When** the form is submitted in either storage mode, **Then** validation prevents the invalid entry from being saved.
3. **Given** multiple accepted entries in either storage mode, **When** the Index page is loaded, **Then** entries are displayed in a consistent and documented order.
4. **Given** the JSON feed is requested in either storage mode, **When** the active roster is available, **Then** `Pages/JsonFeed.cshtml.cs` returns the same active roster collection as the Index page.

---

### Edge Cases

- When the Azure database connection is missing or invalid, the application reports that persistence is unavailable and does not claim that the entry was saved.
- When the Azure database is temporarily unavailable during submission, the application preserves the user's form values and provides an actionable retry message.
- When the Azure database contains no entries, the Index page displays the existing empty-roster state.
- When a user submits duplicate roster information, the application preserves the existing duplicate-entry behavior unless a separate uniqueness rule is approved.
- When the application changes from local execution to Azure execution, storage mode follows the active hosting environment and does not migrate local in-memory entries.
- When an Azure deployment is restarted or scaled, all entries saved to the database remain available.
- When the application runs locally with Azure-looking configuration values but is not hosted in Azure, it continues using in-memory storage rather than accidentally writing to the database.
- When an Azure read fails after saved entries exist, the application reports persistence unavailability and MUST NOT present an empty roster as if no entries existed.
- When the Azure schema cannot be initialized, the application reports persistence unavailability and does not accept a submission as saved.
- When concurrent Azure submissions have the same timestamp, the stable persisted identity determines their order.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST retain the existing Index-page roster form, validation rules, and five roster fields.
- **FR-002**: The system MUST select persistent database storage only when the application is running in the Azure deployment environment.
- **FR-003**: Azure persistence MUST use MySQL In App when the application is running in Azure.
- **FR-004**: When Azure database storage is selected and available, the system MUST save each accepted roster entry before reporting the submission as successful.
- **FR-005**: When Azure database storage is selected, the system MUST load saved roster entries for the Index page after application startup, restart, redeployment, or scale-out.
- **FR-005a**: Azure persistence MUST initialize the required schema idempotently before the first roster read or write, whether initialization occurs during startup or on first use; initialization failure MUST be surfaced as persistence unavailable.
- **FR-005b**: Azure reads MUST surface persistence failure rather than returning an empty roster when the database cannot be read.
- **FR-005c**: Azure writes from multiple application instances MUST preserve every accepted entry, allow duplicates, and return entries ordered by UTC creation timestamp followed by stable persisted identity.
- **FR-006**: When the application runs locally or in Visual Studio, the system MUST use the existing in-memory roster and MUST NOT require or contact the Azure database.
- **FR-007**: The system MUST preserve the existing local behavior in which in-memory entries are lost when the application restarts.
- **FR-008**: The system MUST expose the same roster fields and validation behavior through both storage modes.
- **FR-009**: The system MUST NOT silently fall back from failed Azure database persistence to in-memory storage for a submission.
- **FR-010**: When persistence is unavailable, the system MUST display an understandable error, preserve the user's entered values where possible, and avoid reporting the entry as saved.
- **FR-011**: The system MUST keep database connection details and credentials out of source control and use environment-appropriate configuration.
- **FR-011a**: Persistence diagnostics MUST include only safe structured context such as operation, storage mode, and failure category; they MUST exclude passwords, raw connection strings, and provider exception text containing secrets.
- **FR-012**: The system MUST provide automated tests for storage-mode selection, local in-memory behavior, Azure persistence behavior, failure handling, and consistent Index-page behavior.
- **FR-013**: The system MUST preserve accessible labels, validation messages, keyboard access, and consistent user-facing terminology on the Index page.

### Key Entities

- **Roster Entry**: The five-field user submission already displayed by the application.
- **Roster Collection**: The set of entries available to the Index page in the active storage mode.
- **Storage Mode**: The active local in-memory or Azure-persistent behavior selected from the hosting environment.
- **Persistence Availability**: The state indicating whether the selected Azure database can accept or return roster data.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of valid entries submitted successfully in Azure remain visible after an application restart or redeployment.
- **SC-002**: 100% of local and Visual Studio runs can submit and view roster entries without an Azure database connection.
- **SC-003**: 100% of failed Azure persistence attempts communicate that the entry was not saved and preserve the user's input for retry where possible.
- **SC-004**: Existing roster form validation and display tests pass in both storage modes with no behavior-specific UI divergence.
- **SC-005**: The Index page loads the available roster within 2 seconds for up to 1,000 entries in each supported storage mode under normal operating conditions.
- **SC-006**: Automated tests cover environment-based storage selection, successful persistence, persistence failure, local restart loss, and shared validation behavior.
- **SC-007**: A repeatable warm-path measurement with an already initialized store demonstrates that the Index page loads up to 1,000 entries within 2 seconds in both storage modes; the measurement excludes deployment provisioning but includes the page request, store read, and rendering path.
- **SC-008**: Automated tests verify idempotent schema initialization, read-failure behavior, stable ordering under equal timestamps, JSON feed parity, and safe diagnostic redaction.

## Assumptions

- The existing in-memory roster remains the default behavior for local development and Visual Studio.
- Azure is identified by the application's active hosting environment rather than by merely having database settings present.
- The implementation creates the required schema idempotently before the first Azure roster operation; Azure resource provisioning remains an environment/deployment responsibility.
- MySQL In App is available and configured for the Azure App Service deployment, including the required connection details and database lifecycle.
- The existing Index page remains the primary user interface for creating and viewing entries.
- The existing JSON feed at `Roster26FS001/Pages/JsonFeed.cshtml.cs` must expose the active roster collection consistently after storage changes.
- No local database emulator or local persistence fallback is required for this feature.
- Database credentials and connection details will be supplied through Azure-managed configuration or secrets.
