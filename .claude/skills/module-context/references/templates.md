# Per-Folder Document Templates

## .context/CONTEXT.md Template

```markdown
<!-- AUTO-GENERATED: Header -->
# {Folder Name} — Module Context
**Version**: {commit_hash}
**Generated**: {timestamp}
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
{Deep explanation — not "Handles logic", but "Manages the state machine for user onboarding, enforcing valid transitions and persisting side-effects to the DB".}

### Design Patterns
- **{Pattern}**: {Usage context, e.g., "Singleton for DB connection to ensure pool limit"}

### Data Flow
1. Input enters via `{entry_point}`
2. Validated by `{validator}`
3. Transformed by `{transformer}`
4. Persisted via `{repository}`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

> Only exported or critical types.

```{language}
{type definitions with inline comments on constraints}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `{Service.method(arg)}`
- **Input**: `{Type}`
- **Output**: `{Type}`
- **Behavior**: {validation, errors, side-effects}
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

- **Internal**: [{Module}](../../{module}/.context/CONTEXT.md) — {purpose}
- **External**: `{package}` — {purpose}
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights
{Manual notes — preserved across regeneration}
<!-- END CUSTOM SECTION -->
```

## .context/OVERVIEW.md Template

```markdown
<!-- AUTO-GENERATED: Header -->
# {Folder Name}
> {One-line summary}
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
{Human-readable explanation with analogies. Explain the "Why".}
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
```bash
{installation commands if applicable}
```

### Example
```{language}
{Real, runnable example}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `{method}` | {description} |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting
{Manual section — preserved across regeneration}
<!-- END CUSTOM SECTION -->
```

## .context/metadata.json Structure

```json
{
  "version": "1.1",
  "last_commit": "{hash}",
  "generated_at": "{ISO timestamp}",
  "file_count": 15
}
```
