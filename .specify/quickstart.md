# RosterVideo Quickstart

## Local development in Visual Studio 2026

1. Open `Roster26FS001.slnx` in Visual Studio 2026.
2. Restore the solution and build the `Roster26FS001` project.
3. Run the project using the HTTPS launch profile.
4. Open the home page and submit a roster entry.
5. Confirm the entry appears in the home-page list.
6. Follow the JSON feed link and confirm the same entry is returned as valid JSON.
7. Stop and restart the application, then confirm the in-memory roster is empty.

## Functional verification

- Submit valid values for all five fields.
- Submit with each field blank and verify an associated, understandable validation message.
- Submit values with surrounding whitespace and verify the displayed values are trimmed.
- Submit two identical entries and verify both remain listed.
- Verify the empty home-page state and empty JSON array.
- Verify the JSON feed is reachable directly at `/JsonFeed`.

## Accessibility verification

- Complete the form using keyboard navigation only.
- Confirm every control has a visible label and a visible focus indicator.
- Confirm validation messages are associated with their fields and the summary is announced when appropriate.
- Confirm heading order, link text, color contrast, responsive layout, and status messaging meet the project's basic WCAG 2.2 AA checks.
- Use browser accessibility inspection tools to verify that ARIA attributes are valid, necessary, and do not conflict with native semantics.

## Azure deployment readiness

1. Publish the web project using the standard ASP.NET Core deployment workflow.
2. Configure an Azure App Service running a compatible .NET runtime.
3. Set the production environment and HTTPS configuration through Azure settings.
4. Verify the home page, form submission, roster list, and `/JsonFeed` feed after deployment.
5. Confirm that operators understand roster entries are intentionally lost when the process restarts or the app is redeployed.
