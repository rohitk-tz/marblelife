---
name: code-review
description: Deep, multi-pass code review analyzing security vulnerabilities, logic bugs, performance issues, and maintainability. Use when asked to review code, find bugs, improve code quality, audit a file or directory, or analyze code for issues.
---

# Code Review

Perform a structured four-pass review of the target code.

## Execution

### Phase 1: Context Gathering

1. Identify language, framework, and project conventions (`.editorconfig`, `pyproject.toml`, `package.json`, style guides)
2. Understand purpose from function/class names, docstrings, and imports

### Phase 2: Four-Pass Review

**Pass 1 — Security (Critical):** SQL/NoSQL/command/template injection, XSS, path traversal, auth bypass, IDOR, hardcoded secrets, insecure deserialization, SSRF, weak crypto.

**Pass 2 — Logic & Correctness:** Off-by-one errors, null dereferences, race conditions, resource leaks, exception swallowing, incorrect boolean logic, integer overflow, missing edge cases, deadlock potential, missing `await`.

**Pass 3 — Performance:** N+1 queries, missing pagination, sync I/O in hot paths, unnecessary allocations in loops, missing caching, O(n^2) when O(n) is possible, event loop blocking.

**Pass 4 — Maintainability:** Functions >50 lines, classes >10 methods, nesting >4 levels, magic numbers, code duplication, missing types, dead code, complex conditionals.

For language-specific checks, see [references/language-checks.md](references/language-checks.md).

### Phase 3: Report

Use the output template in [references/output-template.md](references/output-template.md).

Structure findings as: Critical (must fix) → Major → Minor → Suggestions → Positive observations.

Each finding must include: file:line, severity, category, current code, recommended fix, and impact explanation.

## Large Codebases

When too large for one pass:
1. Save progress to a checkpoint JSON (completed/pending directories, findings counts)
2. Output checkpoint summary with `/code-review --continue` instruction

## Arguments

- `$1` — Path to file or directory
- `--focus` — `security`, `logic`, `performance`, `style`, or `all` (default: all)
- `--severity` — Minimum: `critical`, `major`, `minor`, `all` (default: all)
- `--continue` — Resume from checkpoint
- `--incremental` — Only review git-changed files
