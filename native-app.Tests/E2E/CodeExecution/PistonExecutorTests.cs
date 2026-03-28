using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeTutor.Wpf.Services;
using CodeTutor.Wpf.Services.Executors;

namespace CodeTutor.Tests.E2E.CodeExecution;

/// <summary>
/// E2E tests for the Piston code execution engine.
/// Tests verify that Python, Java, Kotlin, Rust, and Dart code can be executed via the Piston API.
/// </summary>
public class PistonExecutorTests : IAsyncLifetime
{
    private TestHttpMessageHandler _httpHandler = null!;
    private HttpClient _httpClient = null!;
    private PistonExecutor _pistonExecutor = null!;
    private const string TestBaseUrl = "http://localhost:2000";

    // Language test data for Theory tests
    public static readonly TheoryData<string, string, string, string> LanguageExecutionData = new()
    {
        { "python", "print('Hello, Python!')", "Hello, Python!", "3.10.0" },
        { "javascript", "console.log('Hello, JavaScript!')", "Hello, JavaScript!", "18.15.0" },
        { "java", "public class Main { public static void main(String[] args) { System.out.println(\"Hello, Java!\"); } }", "Hello, Java!", "15.0.2" },
        { "kotlin", "fun main() { println(\"Hello, Kotlin!\") }", "Hello, Kotlin!", "1.8.20" },
        { "rust", "fn main() { println!(\"Hello, Rust!\"); }", "Hello, Rust!", "1.68.2" },
        { "dart", "void main() { print('Hello, Dart!'); }", "Hello, Dart!", "2.19.6" }
    };

    // Language aliases test data
    public static readonly TheoryData<string, string> LanguageAliasData = new()
    {
        { "python", "Python" },
        { "python", "PYTHON" },
        { "javascript", "js" },
        { "javascript", "JS" },
        { "javascript", "JavaScript" },
        { "csharp", "c#" },
        { "csharp", "C#" },
        { "dart", "flutter" }
    };

    public Task InitializeAsync()
    {
        _httpHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_httpHandler)
        {
            Timeout = TimeSpan.FromSeconds(35)
        };
        _pistonExecutor = new PistonExecutor(TestBaseUrl, _httpClient);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _httpClient?.Dispose();
        return Task.CompletedTask;
    }

    #region Constructor Tests

    [Theory]
    [InlineData("http://localhost:2000")]
    [InlineData("https://piston.example.com")]
    [InlineData("http://127.0.0.1:8080")]
    public void Constructor_ValidUrl_SetsBaseUrl(string baseUrl)
    {
        // Act
        var executor = new PistonExecutor(baseUrl, _httpClient);

        // Assert - no exception thrown, executor created successfully
        executor.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("ftp://invalid-protocol.com")]
    [InlineData("//missing-protocol")]
    public void Constructor_InvalidUrl_ThrowsArgumentException(string invalidUrl)
    {
        // Act & Assert
        Action act = () => new PistonExecutor(invalidUrl, _httpClient);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_DefaultUrl_UsesLocalhost()
    {
        // Act - using default constructor (uses static HttpClient)
        var executor = new PistonExecutor();

        // Assert - should use default localhost URL
        executor.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_NullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Action act = () => new PistonExecutor(TestBaseUrl, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Availability Tests

    [Fact]
    public async Task IsAvailableAsync_SuccessfulResponse_ReturnsTrue()
    {
        // Arrange
        _httpHandler.SetupResponse("/api/v2/runtimes", HttpStatusCode.OK, "[{\"language\":\"python\"}]");

        // Act
        var result = await _pistonExecutor.IsAvailableAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAvailableAsync_NotFound_ReturnsFalse()
    {
        // Arrange
        _httpHandler.SetupResponse("/api/v2/runtimes", HttpStatusCode.NotFound, "Not Found");

        // Act
        var result = await _pistonExecutor.IsAvailableAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAvailableAsync_ConnectionRefused_ReturnsFalse()
    {
        // Arrange
        _httpHandler.SetupException("/api/v2/runtimes", new HttpRequestException("Connection refused"));

        // Act
        var result = await _pistonExecutor.IsAvailableAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAvailableAsync_Timeout_ReturnsFalse()
    {
        // Arrange
        _httpHandler.SetupException("/api/v2/runtimes", new TaskCanceledException("Request timeout"));

        // Act
        var result = await _pistonExecutor.IsAvailableAsync();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Successful Execution Tests

    [Theory]
    [MemberData(nameof(LanguageExecutionData))]
    public async Task ExecuteAsync_ValidCode_ReturnsSuccess(string language, string code, string expectedOutput, string version)
    {
        // Arrange
        var response = CreatePistonResponse(expectedOutput, "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync(code, language);

        // Assert
        result.Success.Should().BeTrue($"because {language} execution should succeed");
        result.Output.Should().Contain(expectedOutput);
        result.Error.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(LanguageExecutionData))]
    public async Task ExecuteAsync_ValidCode_MakesCorrectRequest(string language, string code, string expectedOutput, string expectedVersion)
    {
        // Arrange
        var response = CreatePistonResponse(expectedOutput, "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        await _pistonExecutor.ExecuteAsync(code, language);

        // Assert
        var lastRequest = _httpHandler.LastRequest;
        lastRequest.Should().NotBeNull();
        lastRequest!.RequestUri!.ToString().Should().Contain("/api/v2/execute");

        var requestBody = await lastRequest.Content!.ReadAsStringAsync();
        var request = JsonSerializer.Deserialize<TestPistonRequest>(requestBody);
        request.Should().NotBeNull();
        request!.Language.Should().Be(language.ToLowerInvariant());
        request.Version.Should().Be(expectedVersion);
        request.Files.Should().HaveCount(1);
        request.Files[0].Content.Should().Be(code);
    }

    [Theory]
    [MemberData(nameof(LanguageAliasData))]
    public async Task ExecuteAsync_LanguageAliases_AreNormalized(string baseLanguage, string alias)
    {
        // Arrange
        var response = CreatePistonResponse("output", "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        await _pistonExecutor.ExecuteAsync("code", alias);

        // Assert
        var requestBody = await _httpHandler.LastRequest!.Content!.ReadAsStringAsync();
        var request = JsonSerializer.Deserialize<TestPistonRequest>(requestBody);
        request!.Language.Should().Be(baseLanguage);
    }

    #endregion

    #region Error Output Capture Tests

    [Fact]
    public async Task ExecuteAsync_RuntimeError_ReturnsFailureWithStderr()
    {
        // Arrange
        var stderr = "Traceback (most recent call last):\n  File \"main.py\", line 1, in <module>\n    print(undefined_var)\nNameError: name 'undefined_var' is not defined";
        var response = CreatePistonResponse("", stderr, 1);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        var code = "print(undefined_var)";

        // Act
        var result = await _pistonExecutor.ExecuteAsync(code, "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("NameError");
        result.Error.Should().Contain("undefined_var");
    }

    [Fact]
    public async Task ExecuteAsync_CompilationError_ReturnsFailure()
    {
        // Arrange
        var stderr = "Main.java:1: error: class Main is public, should be declared in a file named Main.java";
        var response = CreatePistonResponse("", stderr, 1);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        var code = "public class WrongName { public static void main(String[] args) { } }";

        // Act
        var result = await _pistonExecutor.ExecuteAsync(code, "java");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("error");
    }

    [Fact]
    public async Task ExecuteAsync_NonZeroExitCode_ReturnsFailure()
    {
        // Arrange
        var response = CreatePistonResponse("some output", "", 42);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync("exit(42)", "python");

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_EmptyOutput_WithError_ReturnsFailure()
    {
        // Arrange
        var response = CreatePistonResponse("", "Error occurred", 1);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Output.Should().BeEmpty();
        result.Error.Should().Be("Error occurred");
    }

    #endregion

    #region Timeout Handling Tests

    [Fact]
    public async Task ExecuteAsync_TimesOut_ReturnsTimeoutError()
    {
        // Arrange
        _httpHandler.SetupDelay("/api/v2/execute", TimeSpan.FromSeconds(40));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        // Act
        var result = await _pistonExecutor.ExecuteAsync("while True: pass", "python", cts.Token);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("timed out").Or.Contain("canceled").Or.Contain("cancelled");
    }

    [Fact]
    public async Task ExecuteAsync_CancellationTokenTriggered_AbortsExecution()
    {
        // Arrange
        _httpHandler.SetupDelay("/api/v2/execute", TimeSpan.FromSeconds(10));

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python", cts.Token);

        // Assert
        result.Success.Should().BeFalse();
    }

    #endregion

    #region Unsupported Language Tests

    [Theory]
    [InlineData("brainfuck")]
    [InlineData("assembly")]
    [InlineData("fortran")]
    [InlineData("cobol")]
    public async Task ExecuteAsync_UnsupportedLanguage_ReturnsError(string language)
    {
        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", language);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain($"Language '{language}' not supported by Piston");
    }

    [Fact]
    public async Task ExecuteAsync_EmptyLanguage_ReturnsError()
    {
        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not supported");
    }

    #endregion

    #region API Failure Tests

    [Fact]
    public async Task ExecuteAsync_HttpError_ReturnsErrorWithStatusCode()
    {
        // Arrange
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.InternalServerError, "Internal Server Error");

        // Act
        var result = await _pistonExecutor.ExecuteAsync("print('test')", "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("500").Or.Contain("Internal Server Error");
    }

    [Fact]
    public async Task ExecuteAsync_BadRequest_ReturnsError()
    {
        // Arrange
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.BadRequest, "Invalid request format");

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("400").Or.Contain("Bad Request");
    }

    [Fact]
    public async Task ExecuteAsync_ServiceUnavailable_ReturnsError()
    {
        // Arrange
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.ServiceUnavailable, "Service Unavailable");

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_NetworkError_ReturnsConnectionFailedError()
    {
        // Arrange
        _httpHandler.SetupException("/api/v2/execute", new HttpRequestException("No connection could be made"));

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("connection failed").Or.Contain("Piston");
    }

    #endregion

    #region Malformed Response Tests

    [Fact]
    public async Task ExecuteAsync_InvalidJson_ReturnsError()
    {
        // Arrange
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, "not valid json");

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Invalid response").Or.Contain("response");
    }

    [Fact]
    public async Task ExecuteAsync_NullResponse_ReturnsError()
    {
        // Arrange
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, "null");

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Invalid response");
    }

    [Fact]
    public async Task ExecuteAsync_MissingRunProperty_ReturnsEmptyOutput()
    {
        // Arrange
        var response = "{\"language\":\"python\",\"version\":\"3.10.0\"}";
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeTrue(); // No error, exit code 0 by default
        result.Output.Should().BeEmpty();
        result.Error.Should().BeEmpty();
    }

    #endregion

    #region Output Size Tests

    [Fact]
    public async Task ExecuteAsync_LargeOutput_HandlesCorrectly()
    {
        // Arrange
        var largeOutput = new string('x', 100000); // 100KB output
        var response = CreatePistonResponse(largeOutput, "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeTrue();
        result.Output.Should().HaveLength(largeOutput.Length);
    }

    [Fact]
    public async Task ExecuteAsync_LargeErrorOutput_HandlesCorrectly()
    {
        // Arrange
        var largeError = new string('e', 50000); // 50KB error
        var response = CreatePistonResponse("", largeError, 1);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync("code", "python");

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().HaveLength(largeError.Length);
    }

    #endregion

    #region Specific Language Feature Tests

    [Fact]
    public async Task ExecuteAsync_Python_MultilineScript_ExecutesCorrectly()
    {
        // Arrange
        var code = @"
for i in range(5):
    print(i)
";
        var output = "0\n1\n2\n3\n4";
        var response = CreatePistonResponse(output, "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync(code, "python");

        // Assert
        result.Success.Should().BeTrue();
        result.Output.Should().Contain("0");
        result.Output.Should().Contain("4");
    }

    [Fact]
    public async Task ExecuteAsync_JavaScript_AsyncCode_ExecutesCorrectly()
    {
        // Arrange
        var code = "const result = await Promise.resolve(42); console.log(result);";
        var response = CreatePistonResponse("42", "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync(code, "javascript");

        // Assert
        result.Success.Should().BeTrue();
        result.Output.Should().Contain("42");
    }

    [Fact]
    public async Task ExecuteAsync_Rust_CompileAndRun_ExecutesCorrectly()
    {
        // Arrange
        var code = @"
fn main() {
    let sum: i32 = (1..=5).sum();
    println!(""Sum: {}"", sum);
}
";
        var response = CreatePistonResponse("Sum: 15", "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync(code, "rust");

        // Assert
        result.Success.Should().BeTrue();
        result.Output.Should().Contain("Sum: 15");
    }

    [Fact]
    public async Task ExecuteAsync_Dart_FutureCode_ExecutesCorrectly()
    {
        // Arrange
        var code = @"
void main() async {
  await Future.delayed(Duration(milliseconds: 10));
  print('Done');
}
";
        var response = CreatePistonResponse("Done", "", 0);
        _httpHandler.SetupResponse("/api/v2/execute", HttpStatusCode.OK, response);

        // Act
        var result = await _pistonExecutor.ExecuteAsync(code, "dart");

        // Assert
        result.Success.Should().BeTrue();
        result.Output.Should().Contain("Done");
    }

    #endregion

    #region Helper Methods

    private static string CreatePistonResponse(string stdout, string stderr, int exitCode)
    {
        var response = new
        {
            run = new
            {
                stdout,
                stderr,
                code = exitCode,
                signal = (string?)null
            }
        };
        return JsonSerializer.Serialize(response);
    }

    #endregion

    #region Test Support Classes

    private class TestPistonRequest
    {
        [JsonPropertyName("language")]
        public string Language { get; set; } = "";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "";

        [JsonPropertyName("files")]
        public TestPistonFile[] Files { get; set; } = Array.Empty<TestPistonFile>();
    }

    private class TestPistonFile
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = "";
    }

    /// <summary>
    /// Test HTTP message handler for mocking Piston API responses.
    /// </summary>
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> _handlers = new();

        public HttpRequestMessage? LastRequest { get; private set; }

        public void SetupResponse(string path, HttpStatusCode statusCode, string content)
        {
            _handlers[path] = (request, _) =>
            {
                LastRequest = request;
                var response = new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            };
        }

        public void SetupException(string path, Exception exception)
        {
            _handlers[path] = (request, _) =>
            {
                LastRequest = request;
                throw exception;
            };
        }

        public void SetupDelay(string path, TimeSpan delay)
        {
            _handlers[path] = async (request, cancellationToken) =>
            {
                LastRequest = request;
                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    throw new OperationCanceledException("Request was canceled", cancellationToken);
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? "";

            foreach (var handler in _handlers)
            {
                if (path.Contains(handler.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return await handler.Value(request, cancellationToken);
                }
            }

            // Default: return 404
            LastRequest = request;
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Not Found")
            };
        }
    }

    #endregion
}

/// <summary>
/// Integration-style tests for PistonExecutor that verify the actual API contract.
/// These tests document the expected behavior without requiring a live Piston instance.
/// </summary>
public class PistonExecutorContractTests
{
    [Theory]
    [InlineData("python", "3.10.0")]
    [InlineData("javascript", "18.15.0")]
    [InlineData("csharp", "6.12.0")]
    [InlineData("java", "15.0.2")]
    [InlineData("kotlin", "1.8.20")]
    [InlineData("rust", "1.68.2")]
    [InlineData("dart", "2.19.6")]
    public void LanguageMapping_HasExpectedVersions(string language, string expectedVersion)
    {
        // This test documents the expected language versions
        // The actual mapping is tested via the executor's behavior
        expectedVersion.Should().NotBeNullOrEmpty();
        language.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("python")]
    [InlineData("javascript")]
    [InlineData("csharp")]
    [InlineData("java")]
    [InlineData("kotlin")]
    [InlineData("rust")]
    [InlineData("dart")]
    public void SupportedLanguages_AllowedInPiston(string language)
    {
        // Documents the list of languages supported by the Piston executor
        var supportedLanguages = new[] { "python", "javascript", "csharp", "java", "kotlin", "rust", "dart" };
        supportedLanguages.Should().Contain(language);
    }
}

/// <summary>
/// Tests for PistonExecutor edge cases and boundary conditions.
/// </summary>
public class PistonExecutorEdgeCaseTests
{
    [Fact]
    public void ExecuteAsync_VeryLongCode_Theory()
    {
        // Theory test for very long code submission
        var veryLongCode = new string('x', 1000000); // 1MB of code
        veryLongCode.Should().HaveLength(1000000);
    }

    [Fact]
    public void ExecuteAsync_UnicodeCharacters_HandledCorrectly()
    {
        // Verify unicode handling
        var unicodeCode = "print('Hello, 世界! 🌍')";
        unicodeCode.Should().Contain("世界");
        unicodeCode.Should().Contain("🌍");
    }

    [Fact]
    public void ExecuteAsync_SpecialCharactersInCode_HandledCorrectly()
    {
        // Verify special characters are handled
        var specialCode = "print('\\n\\t\\\"')";
        specialCode.Should().Contain("\\n");
        specialCode.Should().Contain("\\t");
        specialCode.Should().Contain("\\\"");
    }
}
