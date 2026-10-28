# RosterVideo Research

## Decision: Use the existing .NET 10 Razor Pages application

- **Decision**: Implement the feature in the existing `Roster26FS001` web project using Razor Pages.
- **Rationale**: The repository already targets .NET 10 and contains Razor Pages, shared layout, Bootstrap assets, site CSS, and site JavaScript. This minimizes new dependencies and supports local execution in Visual Studio 2026.
- **Alternatives considered**: Blazor and MVC were rejected because the project is already structured as Razor Pages and the feature does not require a client-side component framework.

## Decision: Use framework and BCL facilities first

- **Decision**: Prefer ASP.NET Core Razor Pages model binding and validation, dependency injection, `System.Text.Json`, and ordinary HTML, CSS, and JavaScript. Use JavaScript only for progressive enhancement or accessibility-related behavior that cannot be handled by native HTML.
- **Rationale**: This satisfies the request to use C# core libraries and web standards as much as possible while keeping the application simple and deployable to Azure.
- **Alternatives considered**: A client-side SPA framework and third-party JSON library were rejected as unnecessary for the required form, list, and feed.

## Decision: Store entries in a singleton in-memory store

- **Decision**: Define a small application service that owns the roster collection and register it for application lifetime. The service must safely support concurrent reads and writes.
- **Rationale**: The specification explicitly requires in-memory storage and states that entries are discarded when the application restarts. A focused service keeps storage behavior separate from page handlers and makes it testable.
- **Alternatives considered**: A database, file storage, and session storage were rejected because persistence is out of scope and would not provide a shared application-wide roster.

## Decision: Provide the feed as a Razor Page with JSON output

- **Decision**: Add a separate Razor Page route that returns the roster as a JSON collection using the platform JSON serializer and a stable field naming contract.
- **Rationale**: The specification requires a separate page and a machine-readable feed. A page handler can return JSON without introducing a separate API project.
- **Alternatives considered**: A separate controller/API project was rejected because it adds structure that is not needed for this small feature.

## Decision: Use semantic HTML and targeted ARIA

- **Decision**: Use labels, headings, fieldsets or grouped sections where appropriate, native form controls, validation summaries, and status text first. Add ARIA attributes only where they improve the semantics or dynamically connect validation and status messages.
- **Rationale**: Native HTML provides the most reliable baseline. Targeted ARIA supports keyboard and assistive-technology users without duplicating or conflicting with browser semantics.
- **Alternatives considered**: ARIA-only form semantics were rejected because they are less robust than native controls.

## Decision: Validate against WCAG 2.2 AA basics

- **Decision**: Plan checks for keyboard access, visible focus, labels, error identification, sufficient color contrast, heading hierarchy, meaningful link text, responsive layout, and status announcements where needed.
- **Rationale**: These checks directly address the requested WCAG and ARIA coverage and align with the project constitution's user-experience requirements.
- **Alternatives considered**: Treating accessibility as a later enhancement was rejected because it is an explicit feature constraint.

## Decision: Support local Visual Studio execution and Azure deployment

- **Decision**: Keep configuration environment-neutral, rely on standard ASP.NET Core hosting, and document local launch plus Azure App Service deployment prerequisites without adding Azure-specific services.
- **Rationale**: The feature must run locally in Visual Studio 2026 and be deployable to Azure while remaining portable.
- **Alternatives considered**: Azure storage or other managed services were rejected because the feature intentionally loses data on restart.
