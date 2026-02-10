# C# (xUnit) Test Template

```csharp
using Xunit;
using Moq;
using FluentAssertions;

namespace YourNamespace.Tests
{
    public class ClassNameTests : IDisposable
    {
        private readonly Mock<IDatabaseService> _mockDatabase;
        private readonly Mock<IExternalApiClient> _mockApiClient;
        private readonly ClassName _sut;

        public ClassNameTests()
        {
            _mockDatabase = new Mock<IDatabaseService>();
            _mockApiClient = new Mock<IExternalApiClient>();
            _sut = new ClassName(_mockDatabase.Object, _mockApiClient.Object);
        }

        public void Dispose() { /* cleanup */ }

        // Constructor
        [Fact]
        public void Constructor_WithValidParams_Initializes()
        {
            var instance = new ClassName("value1", 42);
            instance.Param1.Should().Be("value1");
            instance.IsInitialized.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithInvalidParams_Throws()
        {
            Action act = () => new ClassName(null, -1);
            act.Should().Throw<ArgumentException>();
        }

        // Happy path
        [Fact]
        public void Process_WithValidUser_ReturnsExpected()
        {
            var user = new User { Id = 1, Username = "testuser" };
            var result = _sut.Process(user);
            result.Status.Should().Be("success");
        }

        // Edge cases
        [Fact]
        public void Process_WithNull_Throws()
        {
            Action act = () => _sut.Process(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ProcessBatch_WithEmpty_ReturnsEmpty()
        {
            _sut.ProcessBatch(new List<Item>()).Should().BeEmpty();
        }

        // Parameterized (Theory)
        [Theory]
        [InlineData("valid", true)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        public void ValidateInput_VariousInputs(string input, bool expected)
        {
            _sut.ValidateInput(input).Should().Be(expected);
        }

        [Theory]
        [InlineData(0, "zero")]
        [InlineData(1, "positive")]
        [InlineData(-1, "negative")]
        [InlineData(int.MaxValue, "positive")]
        public void ClassifyNumber_BoundaryValues(int num, string expected)
        {
            _sut.ClassifyNumber(num).Should().Be(expected);
        }

        // Integration with mocks
        [Fact]
        public void Save_CallsDatabase()
        {
            _mockDatabase.Setup(db => db.Save(It.IsAny<object>())).Returns(true);
            _sut.Save(new { id = 1 });
            _mockDatabase.Verify(db => db.Save(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task FetchExternalData_WithTimeout_HandlesGracefully()
        {
            _mockApiClient.Setup(c => c.FetchAsync())
                .ThrowsAsync(new TimeoutException("timeout"));
            var result = await _sut.FetchExternalDataAsync();
            result.Status.Should().Be("error");
        }

        // Async
        [Fact]
        public async Task ProcessAsync_WithValidInput_Resolves()
        {
            var result = await _sut.ProcessAsync("test");
            result.Status.Should().Be("complete");
        }

        [Fact]
        public async Task ProcessAsync_WithNull_Throws()
        {
            Func<Task> act = async () => await _sut.ProcessAsync(null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
```

## Run command
```bash
dotnet test --collect:"XPlat Code Coverage"
```
