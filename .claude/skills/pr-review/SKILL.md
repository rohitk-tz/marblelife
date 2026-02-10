---
name: pr-review
description: Professional pull request review with structured feedback across correctness, security, performance, testing, maintainability, and compatibility. Use when asked to review a PR, check changes, analyze a diff, or review pull request code quality.
---

# PR Review

Perform a thorough, constructive pull request review.

## Execution

### Phase 1: Context

1. Read PR title, description, and linked issues to understand the problem being solved
2. Analyze scope: count changed files, identify change type (feature, fix, refactor, docs)
3. Check existing tests and related files that might need updates

### Phase 2: Six-Dimension Review

**Correctness:** Does it work? Edge cases handled? Error handling? Async correctness? State management?

**Security:** No injection vulnerabilities, hardcoded secrets, or IDOR. Input validation present. Sensitive data not logged. Secure defaults.

**Performance:** No N+1 queries, unnecessary DB calls, or blocking operations in hot paths. Caching considered. Memory efficient.

**Testing:** New code has tests. Happy path, error cases, and edge cases covered. Tests are not flaky.

**Maintainability:** Readable, appropriately sized functions, clear naming, follows project conventions, DRY.

**Compatibility:** No undocumented breaking API changes. Migrations reversible. Dependencies updated appropriately.

For PR-type-specific checklists, see [references/pr-type-checklists.md](references/pr-type-checklists.md).

### Phase 3: Write Review

Use the output template in [references/output-template.md](references/output-template.md).

**Review etiquette:**
- Be specific — "This query could cause X. Consider Y instead." not "This is wrong."
- Explain why — "Using X prevents Y because Z" not "Use X instead."
- Offer alternatives with code examples
- Severity levels: Blocker (must fix) → Suggestion (non-blocking) → Nit (optional)
- Acknowledge good work

## Large PRs

1. Request split if scope is too large
2. Prioritize: security-sensitive → core business logic → public APIs
3. Use checkpoint with `/pr-review --continue` for continuation

## Arguments

- `$1` — PR URL or reference (defaults to current diff)
- `--focus` — `security`, `performance`, `tests`, or `all`
- `--strict` — Stricter review for production code
- `--continue` — Resume large PR review
