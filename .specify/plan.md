# Implementation Plan: RosterVideo

**Branch**: `.specify` | **Date**: 2026-10-27 | **Spec**: [feature-spec.md](feature-spec.md)

**Input**: Feature specification from `.specify/feature-spec.md`

## Summary

Build the RosterVideo feature into the existing .NET 10 Razor Pages application. Users
will submit five required values on the home page, view all accepted entries, and open a
separate JSON feed containing the same data. The implementation will use C# and ASP.NET
Core platform facilities, native HTML, CSS, and minimal JavaScript, with an application-
lifetime in-memory store and no database dependency.

## Technical Context

**Language/Version**: C# with .NET 10

**Primary Dependencies**: ASP.NET Core Razor Pages, built-in model validation,
`System.Text.Json`, existing Bootstrap assets, HTML, CSS, and minimal JavaScript

**Storage**: Application-lifetime in-memory collection owned by a dependency-injected
application service; entries are intentionally lost on restart

**Testing**: .NET test project using MSTest for unit and Razor Pages integration
coverage, plus manual browser accessibility verification

**Target Platform**: Local HTTPS execution in Visual Studio 2026 and Azure App Service

**Project Type**: Server-rendered Razor Pages web application

**Performance Goals**: Home page and JSON feed respond within 2 seconds for up to
1,000 in-memory entries under normal operating conditions

**Constraints**: No persistent storage; preserve submission order; reject invalid
submissions without mutation; use semantic HTML and targeted WCAG 2.2 AA and ARIA
checks; remain deployable without Azure-specific runtime services

**Scale/Scope**: Two user-facing pages, one shared in-memory roster service, five-field
submission model, one JSON feed contract, and focused automated tests

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Code quality**: PASS — use focused Razor Pages, a small storage abstraction, and
  existing project conventions without unnecessary framework additions.
- **Testing standards**: PASS — plan includes validation, service, page behavior, JSON
  contract, and accessibility verification.
- **User experience consistency**: PASS — reuse the existing layout and styling while
  adding consistent labels, validation, navigation, empty-state, and status patterns.
- **Performance requirements**: PASS — bounded field lengths, snapshot reads, no
  database calls, and a documented 1,000-entry response target.
- **Secure and maintainable delivery**: PASS — use server-side validation, encoded
  Razor output, framework JSON serialization, and no secrets or persistent personal data.
- **Technology constraints**: PASS — target .NET 10 Razor Pages and prefer C# core and
  browser platform capabilities.

## Project Structure

### Documentation (this feature)

```text
.specify/
├── feature-spec.md      # Approved feature specification
├── plan.md              # This implementation plan
├── research.md          # Phase 0 decisions and alternatives
├── data-model.md        # Phase 1 data and validation model
├── quickstart.md        # Local, accessibility, and Azure verification
└── contracts/
	└── json-feed.md     # JSON feed contract
```

### Source Code (repository root)

```text
Roster26FS001/
├── Models/
│   └── RosterEntry.cs
├── Services/
│   ├── IRosterStore.cs
│   └── InMemoryRosterStore.cs
├── Pages/
│   ├── Index.cshtml(.cs)          # Form and roster list
│   ├── JsonFeed.cshtml(.cs)       # JSON feed page
│   └── Shared/_Layout.cshtml      # Shared navigation/link updates
└── wwwroot/
	├── css/site.css               # Feature styling and focus states
	└── js/site.js                 # Minimal progressive enhancement only

Roster26FS001.Tests/
├── Services/InMemoryRosterStoreTests.cs
├── Pages/IndexPageTests.cs
└── Pages/JsonFeedPageTests.cs

```

**Structure Decision**: Extend the existing single Razor Pages project with a model and
service layer, retain page-specific UI in `Pages`, reuse the shared layout and static
assets, and add a focused MSTest project for service, page, validation, JSON, and
accessibility-related assertions. The exact test project path will follow the solution's
existing naming convention when tasks are generated.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|---|---|---|
| None | No constitutional violations identified. | The planned structure is the simplest fit for the feature. |

## Post-Design Constitution Check

- **PASS**: The design uses the existing .NET 10 Razor Pages application and keeps the
  feature small, testable, and focused.
- **PASS**: The in-memory store, validation, JSON contract, and page tests provide
  coverage for the required behavior without introducing persistent infrastructure.
- **PASS**: The design explicitly includes semantic HTML, keyboard access, visible focus,
  validation associations, targeted ARIA, and WCAG 2.2 AA checks.
- **PASS**: The design preserves local Visual Studio execution and Azure App Service
  portability without adding an Azure runtime dependency.
