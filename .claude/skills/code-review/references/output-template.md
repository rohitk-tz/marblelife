# Code Review Output Template

```markdown
# Code Review: {file_or_directory}

## Summary
**Overall Assessment:** {emoji} Ready to merge | Needs minor changes | Needs significant work
**Files Reviewed:** {count}
**Issues Found:** {critical} critical, {major} major, {minor} minor

## Critical Issues ({count})

### CR-{N}: {Title}
**File:** `{path}:{lines}`
**Severity:** Critical
**Category:** {Security | Logic | Performance | Maintainability}

**Problem:**
{Description of the issue}

**Current Code:**
```{lang}
{problematic code}
```

**Recommended Fix:**
```{lang}
{fixed code}
```

**Why This Matters:**
{Impact explanation}

---

## Major Issues ({count})
{Same format, reduced detail}

## Minor Issues ({count})
{Brief format: file, current, suggested}

## Suggestions ({count})
{Brief improvement ideas}

## Positive Observations
{What the code does well}

## Review Statistics

| Category | Count |
|----------|-------|
| Security | {n} |
| Logic | {n} |
| Performance | {n} |
| Style | {n} |
```
