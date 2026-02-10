# Python (pytest) Test Template

```python
import pytest
from unittest.mock import Mock, patch, AsyncMock
from {module_path} import {ClassName, function_name}


# Fixtures
@pytest.fixture
def sample_entity():
    return {"id": 1, "name": "test", "email": "test@example.com"}

@pytest.fixture
def mock_database():
    mock_db = Mock()
    mock_db.query.return_value = []
    mock_db.commit.return_value = None
    return mock_db


# Class tests
class TestClassName:

    def test_init_with_valid_params(self):
        instance = ClassName(param1="value1", param2=42)
        assert instance.param1 == "value1"
        assert instance.param2 == 42

    def test_init_with_invalid_type_raises(self):
        with pytest.raises(TypeError, match="param1 must be a string"):
            ClassName(param1=123)

    # Happy path
    def test_method_returns_expected(self, sample_entity):
        instance = ClassName()
        result = instance.process(sample_entity)
        assert result["status"] == "success"

    # Edge cases
    def test_method_with_empty_input(self):
        assert ClassName().process([]) == []

    def test_method_with_none_raises(self):
        with pytest.raises(ValueError, match="Input cannot be None"):
            ClassName().process(None)

    # Mocked integration
    def test_calls_database_correctly(self, mock_database):
        instance = ClassName(db=mock_database)
        instance.save({"id": 1, "name": "test"})
        mock_database.query.assert_called_once()
        mock_database.commit.assert_called_once()

    @patch("module.external_api.fetch")
    def test_handles_api_timeout(self, mock_fetch):
        mock_fetch.side_effect = TimeoutError("Connection timed out")
        result = ClassName().fetch_external_data()
        assert result["status"] == "error"


# Parameterized tests
class TestFunctionName:

    @pytest.mark.parametrize("input_val,expected", [
        ("valid", True),
        ("", False),
        (None, False),
        ("   ", False),
    ])
    def test_with_various_inputs(self, input_val, expected):
        assert function_name(input_val) == expected


# Async tests
class TestAsyncFunctions:

    @pytest.mark.asyncio
    async def test_async_returns_result(self):
        result = await async_function("input")
        assert result["status"] == "complete"

    @pytest.mark.asyncio
    async def test_async_with_mock(self):
        with patch("module.client.fetch", new_callable=AsyncMock) as mock:
            mock.return_value = {"data": "mocked"}
            result = await async_function("input")
            mock.assert_awaited_once()
```

## Run command
```bash
pytest tests/test_{module}.py -v --cov={module}
```
