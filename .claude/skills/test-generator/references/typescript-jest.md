# TypeScript/JavaScript (Jest/Vitest) Test Template

```typescript
import { describe, it, expect, beforeEach, afterEach, vi, Mock } from 'vitest';
// For Jest: import { describe, it, expect, beforeEach, afterEach, jest } from '@jest/globals';
import { ClassName, functionName } from './module';
import { ExternalService } from './services/external';

vi.mock('./services/external', () => ({
  ExternalService: { fetch: vi.fn(), save: vi.fn() },
}));

const createSampleEntity = (overrides = {}) => ({
  id: 1, username: 'testuser', email: 'test@example.com', ...overrides,
});

describe('ClassName', () => {
  let instance: ClassName;

  beforeEach(() => {
    instance = new ClassName();
    vi.clearAllMocks();
  });

  describe('constructor', () => {
    it('should initialize with valid parameters', () => {
      const inst = new ClassName({ param1: 'value', param2: 42 });
      expect(inst.param1).toBe('value');
    });

    it('should throw for invalid parameter types', () => {
      expect(() => new ClassName({ param1: 123 as any })).toThrow(TypeError);
    });
  });

  describe('process', () => {
    it('should return expected result for valid input', () => {
      const result = instance.process(createSampleEntity());
      expect(result).toEqual({ status: 'success', userId: 1 });
    });

    it('should handle empty input', () => {
      expect(instance.process([])).toEqual([]);
    });

    it('should handle null input', () => {
      expect(() => instance.process(null)).toThrow('Input cannot be null');
    });

    it('should handle unicode characters', () => {
      const result = instance.process({ name: '日本語テスト' });
      expect(result.name).toBe('日本語テスト');
    });
  });

  describe('integration', () => {
    it('should handle API timeout gracefully', async () => {
      (ExternalService.fetch as Mock).mockRejectedValue(new Error('timeout'));
      const result = await instance.fetchExternalData();
      expect(result.status).toBe('error');
    });
  });
});

describe('functionName', () => {
  it.each([
    ['valid', true],
    ['', false],
    [null, false],
    ['   ', false],
  ])('should return %s for input "%s"', (input, expected) => {
    expect(functionName(input)).toBe(expected);
  });
});

describe('async functions', () => {
  it('should resolve with expected result', async () => {
    const result = await asyncFunction('input');
    expect(result.status).toBe('complete');
  });

  it('should reject for invalid input', async () => {
    await expect(asyncFunction(null)).rejects.toThrow();
  });
});
```

## Run command
```bash
npm test -- --coverage
```
