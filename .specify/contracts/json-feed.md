# RosterVideo JSON Feed Contract

## Route

- **Path**: `/JsonFeed`
- **Method**: `GET`
- **Authentication**: None for this feature
- **Response**: JSON collection of roster entries

## Response shape

```json
[
  {
	"firstName": "Ada",
	"lastName": "Lovelace",
	"major": "Computer Science",
	"favoriteKeyboardShortcut": "Ctrl+K",
	"shortcutUsage": "Opening the command palette"
  }
]
```

An empty roster returns:

```json
[]
```

## Contract rules

- The feed contains the same five values displayed on the home page.
- Property names use lower camel case.
- Entries retain submission order.
- Duplicate entries are permitted.
- The response is valid JSON for both empty and populated rosters.
- The endpoint must not expose server-only implementation details or generated storage identifiers.
