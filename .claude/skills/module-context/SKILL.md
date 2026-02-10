---
name: module-context
description: Generate and keep up-to-date deep, inter-linked CONTEXT.md (AI/agent context) and OVERVIEW.md (human-readable guide) inside a `.context/` folder within each code module. Detects stale docs via git diff and updates only what changed. Use when documenting a specific module/folder, updating documentation after code changes, creating AI/agent context for a component, or when asked to generate per-folder documentation.
---

# Per-Folder Documentation

Generate and maintain a `.context/` folder inside the target module containing `CONTEXT.md` (AI/agent reference) and `OVERVIEW.md` (human-readable developer guide). The CONTEXT.md should make re-reading the source code unnecessary.

## Output Structure

```
<module>/
├── .context/
│   ├── CONTEXT.md       # AI/agent context — replaces need to read source
│   ├── OVERVIEW.md      # Human-readable module guide
│   └── metadata.json    # Version tracking for staleness detection
├── src files...
└── ...
```

## Execution

### Phase 0: Staleness Check

Always run this first — avoid unnecessary regeneration.

1. Read `.context/metadata.json` inside the target folder
2. Compare stored `last_commit` hash against current state: `git diff --name-only {stored_hash} HEAD -- {target_folder}`
3. **If no changes detected** and `--force` not passed, report "docs are up to date" and stop
4. **If changes detected**, note which files changed — use this to scope the analysis in Phase 1 (focus on changed files but re-read dependencies if interfaces changed)
5. Capture current commit hash for metadata update at the end

### Phase 1: Deep Analysis

Read the code — do not guess.

1. Map the folder structure, identify entry points, core logic, types/models
2. Read all interface/type definitions, complex business logic, and trace imports
3. Synthesize: core entities, public API (inputs/outputs/side-effects), internal state management, gotchas/edge cases

**For updates (docs already exist):** Read the existing `.context/CONTEXT.md` and `.context/OVERVIEW.md` first. Focus analysis on changed files but verify that unchanged sections are still accurate (e.g., a renamed function breaks the existing API table).

### Phase 2: Generate or Update Documents

Create `.context/` directory inside the target folder if missing. Use templates in [references/templates.md](references/templates.md).

**`.context/CONTEXT.md`** — replaces the need to read source code:
- Architectural mental model with design patterns
- Data flow description
- Critical type definitions (exported/critical types only)
- Public interfaces with inputs, outputs, behavior, side-effects
- Internal and external dependency links to other modules' `.context/CONTEXT.md` files

**`.context/OVERVIEW.md`** — human-readable developer guide:
- Overview with analogies and "why"
- Setup and usage examples (real, runnable code)
- API summary table
- Troubleshooting section (preserved across updates via `CUSTOM SECTION` markers)

**Update behavior (when docs already exist):**
- Read existing documents first
- **Preserve** `<!-- CUSTOM SECTION -->` blocks — these contain manual notes that must survive regeneration
- **Update** `<!-- AUTO-GENERATED -->` blocks with current codebase state
- **Add** new sections for new public interfaces, types, or dependencies
- **Remove** sections for deleted interfaces or types (don't leave stale references)
- Verify all dependency links still point to valid `.context/CONTEXT.md` files

### Phase 3: Metadata Update

Write `.context/metadata.json` inside the target folder:

```json
{
  "version": "1.1",
  "last_commit": "{current_HEAD_hash}",
  "generated_at": "{ISO timestamp}",
  "file_count": 15,
  "changed_files": ["service.py", "models.py"]
}
```

## Quality Rules

- Never use: "Contains logic", "Standard implementation", "Helper functions"
- For `utils.ts`: list every utility function and what it solves
- For `types.ts`: paste the core schemas
- For `api.ts`: explain the error handling strategy

## Large Project Strategy

Document in order: leaf nodes (utils, models) → business logic → API controllers.

## Arguments

- `$1` — Target directory
- `--force` — Regenerate all docs regardless of staleness check
- `--recursive` — Document all subfolders
- `--deep` — Full-file reading mode (recommended for first run)
