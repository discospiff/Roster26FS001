# Feature Specification: RosterVideo

**Feature Branch**: `roster-video`

**Created**: 2026-10-27

**Status**: Draft

**Input**: User description: "Using the template format in .specify/templates/spec-template.md, write a functional specification for RosterVideo into .specify/feature-spec.md. The roster should be a web application where, on a form on the home page, users can enter their first name, last name, major, favorite keyboard shortcut, and where that shortcut can be used. Once submitted, this information should be stored in memory. The home page should also show a list of this same information for all users who have already filled out the form. The home page should have a link to a separate page that shows a JSON feed of this same information."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Submit a roster entry (Priority: P1)

As a user, I want to submit my name, major, favorite keyboard shortcut, and the place where I use it so that I can contribute my information to the roster.

**Why this priority**: Collecting a valid roster entry is the primary purpose of the feature and enables all other user value.

**Independent Test**: Complete the home-page form with valid values, submit it, and verify that the new entry appears in the roster list.

**Acceptance Scenarios**:

1. **Given** the home page is displayed, **When** a user enters a first name, last name, major, favorite keyboard shortcut, and where the shortcut can be used, then submits the form, **Then** the entry is accepted and displayed in the roster list.
2. **Given** the form contains missing or invalid required values, **When** the user submits it, **Then** the entry is not added and each invalid field has a clear validation message.
3. **Given** a user has submitted an entry, **When** the user views the home page again during the same application run, **Then** the submitted entry remains available in the roster list.

---

### User Story 2 - View the roster on the home page (Priority: P1)

As a user, I want to see all submitted roster entries on the home page so that I can learn the information provided by other users.

**Why this priority**: The roster must provide immediate shared value after entries are submitted.

**Independent Test**: Add multiple valid entries and verify that the home page displays each entry and all five submitted values for every entry.

**Acceptance Scenarios**:

1. **Given** no entries have been submitted, **When** a user opens the home page, **Then** the page displays the submission form and an understandable empty-roster message.
2. **Given** one or more entries have been submitted, **When** a user opens the home page, **Then** the page lists every entry with the first name, last name, major, favorite keyboard shortcut, and where that shortcut can be used.
3. **Given** multiple entries have been submitted, **When** a user views the roster, **Then** each entry is presented as a distinct item and no submitted entry is omitted.

---

### User Story 3 - Retrieve the roster as JSON (Priority: P2)

As a user or consuming client, I want to open a separate JSON feed so that I can access the same roster information in a machine-readable format.

**Why this priority**: The feed extends the roster's usefulness to integrations and automated consumers while depending on the core roster data.

**Independent Test**: Submit one or more entries, follow the JSON feed link from the home page, and verify that the response contains the same entries and values shown on the home page.

**Acceptance Scenarios**:

1. **Given** the home page is displayed, **When** a user selects the JSON feed link, **Then** the user is taken to a separate page that presents the roster as valid JSON.
2. **Given** roster entries exist, **When** a client retrieves the JSON feed, **Then** the feed contains every roster entry and includes first name, last name, major, favorite keyboard shortcut, and shortcut usage for each entry.
3. **Given** no roster entries exist, **When** a client retrieves the JSON feed, **Then** the feed is valid JSON representing an empty roster.
4. **Given** an entry is submitted successfully, **When** the JSON feed is retrieved, **Then** the new entry is included with the same values as the home-page roster.

---

### Edge Cases

- When the user submits the form with one or more blank required fields, the system rejects the submission and identifies the fields requiring attention.
- When a user enters leading or trailing whitespace, the system ignores that surrounding whitespace when validating and displaying the value.
- When a value exceeds the allowed field length, the system rejects that field with a clear validation message and does not add a partial entry.
- When two users submit identical information, the system retains both submissions because each represents a separate roster entry.
- When the application has no entries, the home page displays an empty-state message and the JSON feed returns an empty collection rather than an error.
- When the application restarts, previously submitted entries are no longer available because roster data is held only in memory.
- When a user navigates directly to the JSON feed, the feed remains accessible without first visiting the home page.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a home page containing a form for first name, last name, major, favorite keyboard shortcut, and where that shortcut can be used.
- **FR-002**: The system MUST require a non-blank value for each of the five form fields before accepting a submission.
- **FR-003**: The system MUST validate submitted values against defined maximum lengths and MUST display an actionable validation message for each invalid field.
- **FR-004**: The system MUST preserve the user's entered values when a submission is rejected so the user can correct only the invalid fields.
- **FR-005**: The system MUST add each valid submission as a new roster entry stored in application memory.
- **FR-006**: The system MUST make a successfully submitted entry available to subsequent home-page views during the same application run.
- **FR-007**: The home page MUST display all roster entries, including each entry's first name, last name, major, favorite keyboard shortcut, and where that shortcut can be used.
- **FR-008**: The home page MUST display a clear empty-state message when no roster entries are available.
- **FR-009**: The home page MUST provide a link to a separate JSON feed page.
- **FR-010**: The JSON feed page MUST return the same roster information as the home page in valid JSON format.
- **FR-011**: The JSON feed MUST represent an empty roster as an empty collection rather than an error.
- **FR-012**: The system MUST make the JSON feed available through direct navigation as well as through the home-page link.
- **FR-013**: The system MUST treat each accepted submission as a distinct entry, including when its values match another entry.
- **FR-014**: The system MUST not claim persistence beyond the current application run; entries are intentionally discarded when the application restarts.
- **FR-015**: User-facing labels, validation messages, empty-state text, roster labels, and navigation MUST use consistent and understandable terminology.

### Key Entities

- **Roster Entry**: A user's submitted roster information, consisting of first name, last name, major, favorite keyboard shortcut, and where that shortcut can be used.
- **Roster Collection**: The set of roster entries currently held in memory during the application run.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: At least 95% of users entering valid values can submit a roster entry and see it in the home-page list on their first attempt.
- **SC-002**: A user can complete the five-field roster submission in under 60 seconds when valid information is readily available.
- **SC-003**: 100% of accepted submissions appear in both the home-page roster and the JSON feed during the same application run.
- **SC-004**: 100% of rejected submissions display an actionable validation message for every invalid field and add no partial roster entry.
- **SC-005**: The home page displays the roster or its empty-state message within 2 seconds for a collection of up to 1,000 entries under normal operating conditions.
- **SC-006**: The JSON feed is valid and can be parsed by a standard JSON consumer for both empty and populated rosters.
- **SC-007**: In usability review, at least 90% of users can locate the submission form and JSON feed link without assistance.

## Assumptions

- RosterVideo is an unauthenticated web application for users who can access the application.
- The five fields are required and are represented as text values; no controlled vocabulary for majors or keyboard shortcuts is specified for this version.
- Maximum field lengths will use practical defaults appropriate for web forms and will be documented during planning.
- In-memory storage is intentional for this feature; database or file persistence is out of scope.
- The JSON feed contains the same five user-provided values and does not include additional personal information or generated identifiers unless required during planning.
- The home page and JSON feed are the only required user-facing views for this feature.
- Entries are available to all users of the application during the current application run; access control is out of scope.
