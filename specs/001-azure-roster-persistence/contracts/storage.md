# Roster Storage Contract

## Application-facing operations

The storage abstraction must support:

- `AddAsync(RosterEntry entry, CancellationToken cancellationToken)`
- `GetAllAsync(CancellationToken cancellationToken)`

Implementations:

- Local/Visual Studio: application-lifetime in-memory storage.
- Azure: MySQL In App storage using the normalized `MYSQLCONNSTR_localdb` connection string.

## Behavioral contract

- Accepted entries are returned in ascending UTC creation timestamp order, with stable persisted identity as the tie-breaker when timestamps are equal.
- Duplicate entries are allowed.
- A successful add is not reported until the selected store has accepted the entry.
- Azure schema initialization is idempotent and completes before the first Azure read or write.
- A failed Azure initialization, read, or write is surfaced as a persistence failure and never silently routed to memory; a failed read must not be represented as an empty roster.
- Local mode never opens a MySQL connection.

## Azure connection normalization contract

Input: `MYSQLCONNSTR_localdb`.

Output: a valid connection string for the selected MySQL client.

Required behavior:

- Parse the supplied key/value connection string rather than concatenating untrusted text.
- Map Azure In App key names to the client provider's canonical server, port, database, user, and password keys.
- Preserve the exact generated port contained in `MYSQLCONNSTR_localdb`.
- Reject a missing or invalid port; never assume or inject port 3306.
- Do not expose password values in exceptions, logs, test output, or diagnostics.

## Diagnostics

- Persistence diagnostics may include the operation, storage mode, and failure category.
- Passwords, raw connection strings, and secret-bearing provider exception details must not appear in exceptions, logs, responses, test output, or diagnostics.

## Database shape

The implementation must define an idempotent table for the RosterEntry fields, a generated stable identity, a UTC creation timestamp, and indexes/order support for the documented ordering rule. Existing data must survive application restarts, redeployments, and scale-out, including concurrent submissions from multiple application instances.
