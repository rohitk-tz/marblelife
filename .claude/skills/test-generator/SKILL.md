---
name: test-generator
description: Generate comprehensive unit tests with edge cases, mocks, fixtures, and parameterized tests. Supports Python (pytest), TypeScript/JavaScript (Jest/Vitest), Go (testing), Java (JUnit 5), and C# (xUnit). Use when asked to create tests, write unit tests, generate test coverage, add tests for a function/class, or improve test coverage.
---

# Test Generator

Generate production-quality tests for the target code.

## Execution

### Phase 1: Analyze Target Code

1. Identify testable units: public functions, methods, class constructors, API endpoints
2. Identify dependencies to mock: DB connections, external APIs, file system, time/date, random
3. Detect framework from project config (`pytest.ini`, `jest.config`, `go.mod`, `pom.xml`, etc.)

### Phase 2: Plan Test Scenarios

For each testable unit, plan tests across four categories:

**Happy path:** Valid inputs produce expected outputs, correct state changes, proper return types, expected side effects.

**Edge cases:** Empty inputs, null/None, single-element collections, boundary values (0, -1, MAX_INT), unicode, whitespace, very long strings, deeply nested structures.

**Error cases:** Invalid input types, missing required params, out-of-range values, malformed data, network/IO failures, timeouts, permission errors.

**Integration (with mocks):** DB operations, external API calls, file system, cache behavior.

### Phase 3: Generate Tests

Use framework-appropriate patterns. For templates with fixtures, mocking, parameterized tests, and async patterns, see the relevant reference file:

- **Python (pytest):** [references/python-pytest.md](references/python-pytest.md)
- **TypeScript/JavaScript (Jest/Vitest):** [references/typescript-jest.md](references/typescript-jest.md)
- **Go (testing):** [references/go-testing.md](references/go-testing.md)
- **Java (JUnit 5):** [references/java-junit.md](references/java-junit.md)
- **C# (xUnit):** [references/csharp-xunit.md](references/csharp-xunit.md)

**General principles:**
- One assert per concept (not per test)
- Descriptive test names that explain the scenario
- Arrange-Act-Assert structure
- Use fixtures/factories for test data
- Mock external dependencies, not the unit under test
- Use parameterized tests for input variations

### Phase 4: Output

Report: framework used, test file path, coverage target, test counts by category, the generated code, coverage estimation, and run command.

## Large Files

Split by class/function groups. Prioritize public API. Use checkpoint with `/test-generator --continue` for continuation.

## Arguments

- `$1` — Path to file to generate tests for
- `--framework` — Force: `pytest`, `jest`, `vitest`, `go`, `junit`, `xunit`
- `--coverage` — Target coverage percentage (default: 80)
- `--output` — Output path for test file
- `--continue` — Resume from checkpoint
