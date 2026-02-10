---
name: project-context
description: Generate and keep up-to-date project-level "Source of Truth" documentation inside a `.context/` folder — CONTEXT.md (AI/agent system context), ARCHITECTURE.md (system blueprint with diagrams), and the root README.md. Detects stale docs via git diff and updates only what changed. Use when initializing AI context for a project, onboarding new contributors, creating project documentation, updating project docs after changes, or when asked to document the overall project structure and architecture.
---

# Project Documentation

Generate and maintain project-level documentation inside `.context/` at the repo root, plus keep the root `README.md` in sync with the codebase.

## Output Structure

```
<project-root>/
├── .context/
│   ├── CONTEXT.md        # AI/agent system context (agent-agnostic)
│   ├── ARCHITECTURE.md   # System blueprint with diagrams
│   └── metadata.json     # Version tracking for staleness detection
└── README.md             # Human onboarding
```

## Execution

### Phase 0: Staleness Check

Always run this first — avoid unnecessary regeneration.

1. Read `.context/metadata.json` if it exists
2. Compare stored `last_commit` hash against current HEAD: `git diff --name-only {stored_hash} HEAD`
3. **If no changes detected** and `--force` not passed, report "docs are up to date" and stop
4. **If changes detected**, note which top-level directories/files changed — use this to scope updates in Phase 2
5. Capture current commit hash for metadata update at the end

### Phase 1: Reconnaissance

1. Identify build system, database, and entry points from config files (`package.json`, `pom.xml`, `pyproject.toml`, `docker-compose.yml`, etc.)
2. Map top-level modules via directory listing (depth 2)
3. Read the existing `.context/CONTEXT.md`, `.context/ARCHITECTURE.md`, and `README.md` to understand current state

### Phase 2: Generate or Update Documents

Create `.context/` directory at project root if missing. Use templates in [references/templates.md](references/templates.md) for structure.

| File | Location | Audience | Purpose |
|------|----------|----------|---------|
| `CONTEXT.md` | `.context/` | AI agents / tools | Full system context: architecture, module map with links to per-folder `.context/CONTEXT.md`, dev standards, quick-reference snippets |
| `ARCHITECTURE.md` | `.context/` | Both | System blueprint: Mermaid diagrams, data flows, security design |
| `README.md` | repo root | Humans | Landing page: what it is, prerequisites, quick start, links to service docs |

**Update behavior (when docs already exist):**
- Read the existing document first
- Compare against current codebase state
- **Preserve** the existing structure and custom content
- **Update** sections that are stale (e.g., new modules missing from module map, changed endpoints, new env vars, updated commands)
- **Add** sections that are missing but should exist
- **Do not** remove custom content the user may have added
- For `README.md` specifically: update API endpoints, env vars, project structure, commands, and tech stack if they've changed. Preserve custom prose, badges, and sections the user authored.

**First-time generation (when docs don't exist):**
- If `README.md` already exists and is comprehensive, preserve its structure — update stale sections only
- If `README.md` is missing or minimal, generate one using the template
- For the reference README structure of a well-documented project, see [references/readme-reference.md](references/readme-reference.md)

**Key principles:**
- Map, not territory — explain patterns, relationships, and intent. Never duplicate code.
- Link to per-folder `.context/CONTEXT.md` files for deep dives (create stubs if missing)
- Include only commands that actually exist in the project
- All generated files are agent-agnostic — no tool-specific naming

### Phase 3: Validation

1. Verify all relative links point to real files/folders
2. Confirm quick-start commands match actual scripts in `package.json`, `Makefile`, etc.
3. Check that module navigation table includes all current top-level modules

### Phase 4: Metadata Update

Write `.context/metadata.json`:

```json
{
  "version": "1.0",
  "last_commit": "{current_HEAD_hash}",
  "generated_at": "{ISO timestamp}",
  "files_generated": ["CONTEXT.md", "ARCHITECTURE.md"],
  "readme_updated": true
}
```

## Arguments

- `$1` — Project root path
- `--deep` — Read deeper into submodules (slower, more thorough)
- `--skeleton` — Create file structure with TODOs only
- `--readme-only` — Only update the README.md
- `--force` — Regenerate all docs regardless of staleness check
