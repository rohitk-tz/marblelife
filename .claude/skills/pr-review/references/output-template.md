# PR Review Output Template

```markdown
# Pull Request Review

## Overview

**PR Title:** {title}
**Author:** @{author}
**Branch:** `{branch}` -> `{target}`

| Metric | Value |
|--------|-------|
| Files Changed | {n} |
| Additions | +{n} |
| Deletions | -{n} |
| Commits | {n} |

## Overall Assessment

**Status:** Approved | Approved with Comments | Changes Requested

**Summary:**
{2-3 sentences on overall quality and key concerns}

---

## Required Changes (Blockers)

### RC-{N}: {Title}
**File:** `{path}:{line}`
**Category:** {Security | Reliability | Correctness}

{Description of the issue}

```diff
- {problematic code}
+ {suggested fix}
```

**Why this matters:**
{Impact explanation}

---

## Suggestions (Non-Blocking)

### S-{N}: {Title}
**File:** `{path}:{line}`
**Category:** {Performance | Maintainability}

{Description and suggested alternative with code example}

---

## Nits (Optional)

### N-{N}: {Title}
**File:** `{path}:{line}`

```diff
- {current}
+ {suggested}
```

---

## Positive Highlights

1. {What the PR does well}

---

## Testing Recommendations

- [ ] {Suggested test to add}

---

## Questions for Author

1. {Clarifying questions}

---

## Summary

{Final assessment and next steps}

## Checklist

- [ ] Required changes addressed
- [ ] Tests pass
- [ ] Documentation updated
- [ ] Ready for merge
```
