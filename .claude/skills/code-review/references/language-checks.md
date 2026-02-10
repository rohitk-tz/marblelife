# Language-Specific Checks

## Python
- `==` for None comparison (use `is None`)
- Mutable default arguments `def f(x=[]):`
- Not using context managers (`with open(...)`)
- Bare except clauses `except:`
- `type()` instead of `isinstance()`
- Import order violations (stdlib, third-party, local)
- Missing `__all__` in public modules
- Circular imports
- `assert` for validation (disabled with `-O`)

## TypeScript / JavaScript
- Using `any` type (should be specific)
- Not awaiting promises
- Memory leaks in event listeners
- `==` instead of `===`
- React: missing dependency arrays in `useEffect`
- React: mutating state directly
- Unhandled promise rejections
- `var` instead of `let/const`
- Missing optional chaining `?.`

## Go
- Unchecked error returns
- Goroutine leaks (missing context cancellation)
- Data races (shared state without sync)
- `defer` in loops
- `panic` for errors (should return error)
- Not using `errors.Is`/`errors.As`
- Nil pointer dereference
- Unclosed response bodies
- `fmt.Sprint` in hot paths

## Java
- Not using try-with-resources
- Catching `Exception` instead of specific types
- Raw types instead of generics
- Mutable objects as map keys
- Missing `hashCode` when overriding `equals`
- `Date` instead of `java.time`
- String concatenation in loops (use `StringBuilder`)
- Exposing mutable internal state
