# Azure Roster Persistence Quickstart

## Local and Visual Studio verification

1. Run the application locally without Azure MySQL configuration.
2. Submit a valid roster entry on the Index page.
3. Confirm the entry appears in the existing in-memory roster.
4. Confirm no MySQL connection is attempted.
5. Restart the application and confirm the local entry is gone.
6. Run the existing automated test suite without requiring Azure resources.

## Azure configuration verification

1. Configure MySQL In App for the Azure App Service.
2. Confirm Azure exposes `MYSQLCONNSTR_localdb` to the application.
3. Confirm the value contains the generated host and port.
4. Verify that the application normalizes the value without replacing the generated port with 3306.
5. Confirm database credentials are supplied through Azure-managed configuration and are not logged.
6. Deploy the application and submit a valid roster entry.
7. Restart or redeploy the application and verify that the entry remains available.
8. Confirm the JSON feed, if enabled, returns the same persisted roster collection.

## Failure verification

- Remove or corrupt `MYSQLCONNSTR_localdb` in a controlled environment and verify startup or the first database operation reports a safe configuration error.
- Make the database unavailable during submission and verify the user sees an actionable error, the entry is not reported as saved, and the form values remain available for retry where supported.
- Confirm Azure failures never silently write the entry only to memory.

## Test layers

- Unit tests: connection-string normalization, generated-port preservation, malformed values,
  storage-mode selection, and in-memory behavior.
- Component/page tests: shared validation, error display, roster ordering, and store selection.
- Opt-in integration tests: MySQL schema initialization, add/read persistence, restart
  continuity, and failure handling against a configured test database.
