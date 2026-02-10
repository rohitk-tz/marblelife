# Go (testing) Test Template

```go
package module_test

import (
    "context"
    "errors"
    "testing"

    "github.com/stretchr/testify/assert"
    "github.com/stretchr/testify/mock"
    "github.com/stretchr/testify/require"

    "your/module"
)

// Mocks
type MockDatabase struct {
    mock.Mock
}

func (m *MockDatabase) Save(ctx context.Context, data interface{}) error {
    return m.Called(ctx, data).Error(0)
}

// Fixtures
func createSampleUser() *module.User {
    return &module.User{ID: 1, Username: "testuser", Email: "test@example.com"}
}

// Constructor tests
func TestClassName_New(t *testing.T) {
    t.Run("creates instance with valid params", func(t *testing.T) {
        instance, err := module.NewClassName("value", 42)
        require.NoError(t, err)
        assert.Equal(t, "value", instance.Param1)
    })

    t.Run("returns error for invalid params", func(t *testing.T) {
        _, err := module.NewClassName("", -1)
        assert.Error(t, err)
    })
}

// Method tests
func TestClassName_Process(t *testing.T) {
    t.Run("returns expected result for valid input", func(t *testing.T) {
        instance, _ := module.NewClassName("test", 1)
        result, err := instance.Process(createSampleUser())
        require.NoError(t, err)
        assert.Equal(t, "success", result.Status)
    })

    t.Run("handles nil input", func(t *testing.T) {
        instance, _ := module.NewClassName("test", 1)
        _, err := instance.Process(nil)
        assert.Error(t, err)
    })
}

// Table-driven tests
func TestFunctionName(t *testing.T) {
    tests := []struct {
        name     string
        input    string
        expected bool
        wantErr  bool
    }{
        {"valid input", "valid", true, false},
        {"empty input", "", false, false},
        {"whitespace only", "   ", false, false},
        {"unicode", "日本語", true, false},
    }

    for _, tt := range tests {
        t.Run(tt.name, func(t *testing.T) {
            result, err := module.FunctionName(tt.input)
            if tt.wantErr {
                assert.Error(t, err)
                return
            }
            require.NoError(t, err)
            assert.Equal(t, tt.expected, result)
        })
    }
}

// Integration with mocks
func TestClassName_WithDatabase(t *testing.T) {
    t.Run("calls database correctly", func(t *testing.T) {
        mockDB := new(MockDatabase)
        mockDB.On("Save", mock.Anything, mock.Anything).Return(nil)
        instance := module.NewClassNameWithDB(mockDB)
        err := instance.Save(context.Background(), map[string]interface{}{"id": 1})
        require.NoError(t, err)
        mockDB.AssertCalled(t, "Save", mock.Anything, mock.Anything)
    })

    t.Run("handles database error", func(t *testing.T) {
        mockDB := new(MockDatabase)
        mockDB.On("Save", mock.Anything, mock.Anything).Return(errors.New("connection failed"))
        instance := module.NewClassNameWithDB(mockDB)
        err := instance.Save(context.Background(), nil)
        assert.Error(t, err)
    })
}

// Benchmarks
func BenchmarkFunctionName(b *testing.B) {
    for i := 0; i < b.N; i++ {
        module.FunctionName("test-input")
    }
}
```

## Run command
```bash
go test -v -cover ./...
```
