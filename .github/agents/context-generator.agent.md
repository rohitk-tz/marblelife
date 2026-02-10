---
description: "Generate and maintain AI/agent context documentation (.context/ folders) at both project and module level using the project-context and module-context skills."
---

## User Input

```text
$ARGUMENTS
```

Consider the user input before proceeding (if not empty). The user may specify: specific modules to document, `--force` to regenerate regardless of staleness, `--project-only` or `--modules-only` to scope the run, or a list of folder paths.

## Goal

Generate and maintain `.context/` folders throughout the repository by invoking the **project-context** and **module-context** skills defined in `.claude/skills/`.

## Execution

### Step 1: Determine Scope

- If user passed `--project-only`, skip module-level generation
- If user passed `--modules-only`, skip project-level generation
- Otherwise, run both

### Step 2: Project-Level Context

Invoke the **project-context** skill (`.claude/skills/project-context/SKILL.md`).

Read and follow the full instructions in that skill, including:
- Its staleness detection via `metadata.json`
- Its reference templates at `.claude/skills/project-context/references/templates.md`
- Its README reference at `.claude/skills/project-context/references/readme-reference.md`

Pass through `--force` if the user specified it.

This generates/updates:
- `.context/CONTEXT.md` — AI/agent system context
- `.context/ARCHITECTURE.md` — system blueprint with Mermaid diagrams
- `.context/metadata.json` — version tracking
- `README.md` — human onboarding (update, don't overwrite)

### Step 3: Module-Level Context

Invoke the **module-context** skill (`.claude/skills/module-context/SKILL.md`).

Read and follow the full instructions in that skill, including:
- Its staleness detection via per-module `metadata.json`
- Its reference templates at `.claude/skills/module-context/references/templates.md`

If the user specified specific folders, document only those. Otherwise, identify all significant code folders (skip `node_modules`, `__pycache__`, `.git`, `dist`, `build`, `migrations`, `static`, `assets`, vendor directories).

Pass through `--force` and `--recursive` if the user specified them.

This generates/updates per module:
- `.context/CONTEXT.md` — AI/agent context
- `.context/OVERVIEW.md` — human-readable guide
- `.context/metadata.json` — version tracking

### Step 4: Report

Output a summary:

```markdown
## Context Generation Report

**Commit:** {hash}
**Modules documented:** {count}

### Project Level
- .context/CONTEXT.md — {created|updated|up-to-date}
- .context/ARCHITECTURE.md — {created|updated|up-to-date}
- README.md — {updated|preserved|created}

### Module Level
| Module | Status | Changed Files |
|--------|--------|---------------|
| src/module-a/ | Updated | service.py, models.py |
| src/module-b/ | Created | — |
| src/module-c/ | Up to date | — |
```
