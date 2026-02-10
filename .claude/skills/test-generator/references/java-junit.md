# Java (JUnit 5) Test Template

```java
package com.example.module;

import org.junit.jupiter.api.*;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.ValueSource;
import org.mockito.*;
import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

class ClassNameTest {

    @Mock private DatabaseService mockDatabase;
    @Mock private ExternalApiClient mockApiClient;
    @InjectMocks private ClassName classUnderTest;
    private AutoCloseable closeable;

    @BeforeEach
    void setUp() { closeable = MockitoAnnotations.openMocks(this); }

    @AfterEach
    void tearDown() throws Exception { closeable.close(); }

    // Constructor
    @Test
    @DisplayName("Should initialize with valid parameters")
    void testConstructorWithValidParams() {
        ClassName instance = new ClassName("value1", 42);
        assertEquals("value1", instance.getParam1());
        assertTrue(instance.isInitialized());
    }

    @Test
    @DisplayName("Should throw for invalid parameters")
    void testConstructorThrows() {
        assertThrows(IllegalArgumentException.class, () -> new ClassName(null, -1));
    }

    // Happy path
    @Test
    @DisplayName("Process returns expected result")
    void testProcess() {
        User user = new User(1L, "testuser", "test@example.com");
        Result result = classUnderTest.process(user);
        assertEquals("success", result.getStatus());
    }

    // Edge cases
    @Test
    @DisplayName("Process handles null input")
    void testProcessNull() {
        assertThrows(IllegalArgumentException.class, () -> classUnderTest.process(null));
    }

    @Test
    @DisplayName("ProcessBatch handles empty list")
    void testProcessBatchEmpty() {
        assertTrue(classUnderTest.processBatch(List.of()).isEmpty());
    }

    // Parameterized
    @ParameterizedTest
    @CsvSource({"valid, true", "also_valid, true", "'', false", "' ', false"})
    @DisplayName("ValidateInput with various inputs")
    void testValidateInput(String input, boolean expected) {
        assertEquals(expected, classUnderTest.validateInput(input));
    }

    @ParameterizedTest
    @ValueSource(ints = {0, 1, -1, Integer.MAX_VALUE, Integer.MIN_VALUE})
    @DisplayName("ClassifyNumber handles boundary values")
    void testClassifyNumber(int num) {
        assertNotNull(classUnderTest.classifyNumber(num));
    }

    // Integration with mocks
    @Test
    @DisplayName("Save calls database correctly")
    void testSave() {
        when(mockDatabase.save(any())).thenReturn(true);
        classUnderTest.save(Map.of("id", 1));
        verify(mockDatabase, times(1)).save(any());
    }

    @Test
    @DisplayName("Handles API timeout")
    void testApiTimeout() {
        when(mockApiClient.fetch()).thenThrow(new TimeoutException("timeout"));
        Result result = classUnderTest.fetchExternalData();
        assertEquals("error", result.getStatus());
    }
}
```

## Run commands
```bash
# Maven
mvn test

# Gradle
./gradlew test --info
```
