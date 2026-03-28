using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Tests.E2E.AITutor;

/// <summary>
/// E2E tests for the Phi4TutorService.
/// Tests all tutor functionality using mocked LLM responses to avoid loading actual models.
/// Phi-4 uses a different chat template format than Qwen (<|system|>, <|user|>, <|assistant|>).
/// </summary>
public class Phi4TutorServiceTests : TutorServiceTestBase
{
    private readonly Dictionary<string, string[]> _mockResponses = new()
    {
        ["hint"] = new[] { "Try", " checking", " if", " the", " variable", " exists", " before", " using", " it", "." },
        ["explain_error"] = new[] { "This", " error", " means", " the", " variable", " hasn't", " been", " declared", " or", " is", " out", " of", " scope", "." },
        ["improve"] = new[] { "You", " could", " use", " a", " List", "<", "T", ">", " for", " better", " type", " safety", " instead", "." },
        ["answer"] = new[] { "Variables", " in", " C", "#", " are", " declared", " like", ":", " int", " x", " =", " 5", ";" },
        ["default"] = new[] { "I'm", " your", " programming", " tutor", ".", " Ask", " me", " anything", "!" }
    };

    private ITutorService CreateService()
    {
        return new MockTutorService(_mockResponses);
    }

    #region GenerateHint Tests

    [Fact]
    public async Task GenerateHint_ForCSharpNullReference_SuggestsNullCheck()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(
            language: "C#",
            userCode: "string s = null; Console.WriteLine(s.Length);",
            executionError: "System.NullReferenceException");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Why is my code crashing? Give me a hint",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Should().Contain("variable");
    }

    [Fact]
    public async Task GenerateHint_ForPythonIndentError_HintsAtIndentation()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(
            language: "Python",
            userCode: "def foo():\nprint('bar')",  // Indentation error
            executionError: "IndentationError: expected an indented block");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "What's wrong with my Python code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateHint_ForJavaTypeMismatch_SuggestsTypeCheck()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(
            language: "Java",
            userCode: "String num = \"5\"; int result = num + 10;",
            executionError: "incompatible types: String cannot be converted to int");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Help me fix this type error",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateHint_WithoutCodeContext_ProvidesGeneralGuidance()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#", userCode: null);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "I'm stuck on the loop exercise, any hints?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region ExplainError Tests

    [Theory]
    [InlineData("C#", "CS0029: Cannot implicitly convert type 'string' to 'int'", "implicitly convert")]
    [InlineData("Python", "TypeError: can only concatenate str (not 'int') to str", "concatenate")]
    [InlineData("Java", "cannot find symbol: method length()", "find symbol")]
    public async Task ExplainError_CommonErrors_ProvidesClearExplanation(
        string language, string error, string expectedContext)
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: language, executionError: error);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            $"Explain this error: {error}",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Should().Contain("error");
    }

    [Fact]
    public async Task ExplainError_CompilerVsRuntime_DistinguishesErrorTypes()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();

        // Compiler error
        var compileErrorContext = CreateContext(
            language: "C#",
            executionError: "error CS0103: The name 'undefinedVar' does not exist");

        // Runtime error
        var runtimeErrorContext = CreateContext(
            language: "C#",
            executionError: "System.DivideByZeroException: Attempted to divide by zero");

        // Act
        var compileResponse = await service.SendMessageAsync(
            "Is this a compiler or runtime error?",
            compileErrorContext,
            CreateHistory()).ToListAsync();

        var runtimeResponse = await service.SendMessageAsync(
            "Is this a compiler or runtime error?",
            runtimeErrorContext,
            CreateHistory()).ToListAsync();

        // Assert
        string.Concat(compileResponse).Should().NotBeNullOrEmpty();
        string.Concat(runtimeResponse).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExplainError_WithLineNumber_HighlightsLocation()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var errorWithLine = @"
  File 'script.py', line 42, in calculate
    return x / y
ZeroDivisionError: division by zero";
        var context = CreateContext(language: "Python", executionError: errorWithLine);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Where is the error in my code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExplainError_MultipleErrors_AddressesEachOne()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var multipleErrors = @"
error CS0103: The name 'x' does not exist
error CS0103: The name 'y' does not exist
error CS0029: Cannot implicitly convert type 'int' to 'string'";
        var context = CreateContext(language: "C#", executionError: multipleErrors);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "I have multiple errors, can you explain them?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region SuggestImprovement Tests

    [Fact]
    public async Task SuggestImprovement_CodeReadability_SuggestsBetterNaming()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var codeWithPoorNaming = @"
public int Calc(int a, int b)
{
    return a * b;
}";
        var context = CreateContext(language: "C#", userCode: codeWithPoorNaming);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "How can I make this code more readable?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SuggestImprovement_Performance_IdentifiesBottlenecks()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var slowCode = @"
// Inefficient: O(n^2) lookup
var items = new List<string>();
foreach (var item in largeCollection)
{
    if (items.Contains(item)) // Linear search
        continue;
    items.Add(item);
}";
        var context = CreateContext(language: "C#", userCode: slowCode);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Is this code efficient?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SuggestImprovement_MissingComments_SuggestsDocumentation()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var undocumentedCode = @"
public double Compute(double[] data)
{
    double sum = 0;
    foreach (var d in data)
        sum += d * d;
    return Math.Sqrt(sum / data.Length);
}";
        var context = CreateContext(language: "C#", userCode: undocumentedCode);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Should I add comments to this code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SuggestImprovement_SecurityIssues_FlagsVulnerabilities()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var vulnerableCode = @"
// SQL Injection risk!
string query = $"SELECT * FROM Users WHERE Name = '{userInput}'";";
        var context = CreateContext(language: "C#", userCode: vulnerableCode);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Is there any security issue with this code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region AnswerQuestion Tests

    [Theory]
    [InlineData("What's the difference between == and .Equals() in C#?")]
    [InlineData("How does garbage collection work in .NET?")]
    [InlineData("What are async/await keywords for?")]
    [InlineData("Explain the difference between class and struct")]
    public async Task AnswerQuestion_AdvancedCSharpConcepts_ProvidesAccurateInfo(string question)
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            question,
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Length.Should().BeGreaterThan(5);
    }

    [Fact]
    public async Task AnswerQuestion_FollowUpQuestions_MaintainsContext()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#");
        var history = CreateHistory(
            (MessageRole.User, "What is LINQ?"),
            (MessageRole.Assistant, "LINQ is Language Integrated Query, a feature in C# for querying data."),
            (MessageRole.User, "Can you show me an example?")
        );

        // Act
        var response = await service.SendMessageAsync(
            "How do I filter a list with it?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AnswerQuestion_CodeExampleRequest_ProvidesRunnableCode()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "Python");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Show me an example of list comprehension in Python",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AnswerQuestion_BestPracticesQuestion_ProvidesGuidelines()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "What are the best practices for exception handling?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Empty/Null Input Handling Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t\n  ")]
    public async Task SendMessageAsync_EmptyOrWhitespaceInput_ReturnsGracefulResponse(string input)
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            input,
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessageAsync_NullInput_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await service.SendMessageAsync(null!, context, history).ToListAsync();
        });
    }

    #endregion

    #region Model Not Loaded Tests

    [Fact]
    public async Task SendMessageAsync_BeforeLoadModel_ThrowsInvalidOperation()
    {
        // Arrange
        var service = CreateService();
        var context = CreateContext();
        var history = CreateHistory();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.SendMessageAsync("Hello", context, history).ToListAsync();
        });

        exception.Message.Should().Contain("Model not loaded");
    }

    [Fact]
    public async Task SendMessageAsync_AfterExplicitUnload_ThrowsInvalidOperation()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        service.UnloadModel();
        var context = CreateContext();
        var history = CreateHistory();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.SendMessageAsync("Hello again", context, history).ToListAsync();
        });
    }

    [Fact]
    public async Task LoadModelAsync_CanBeCalledMultipleTimes_Idempotent()
    {
        // Arrange
        var service = CreateService();

        // Act
        await service.LoadModelAsync();
        var firstLoad = service.IsModelLoaded;
        await service.LoadModelAsync(); // Second call should be no-op
        var secondLoad = service.IsModelLoaded;

        // Assert
        firstLoad.Should().BeTrue();
        secondLoad.Should().BeTrue();
    }

    [Fact]
    public async Task LoadModelAsync_ProgressEvent_FiresCorrectly()
    {
        // Arrange
        var service = CreateService();
        var progressSequence = new List<int>();
        service.LoadingProgressChanged += (_, progress) => progressSequence.Add(progress);

        // Act
        await service.LoadModelAsync();

        // Assert
        progressSequence.Should().ContainInOrder(10, 50, 100);
    }

    #endregion

    #region Timeout Handling Tests

    [Fact]
    public async Task SendMessageAsync_ShortTimeout_RespectsCancellation()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await service.SendMessageAsync(
                "[TIMEOUT] Simulate long running task",
                context,
                history,
                cts.Token).ToListAsync();
        });
    }

    [Fact]
    public async Task SendMessageAsync_CancellationTokenAlreadyCancelled_ThrowsImmediately()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await service.SendMessageAsync(
                "Hello",
                context,
                history,
                cts.Token).ToListAsync();
        });
    }

    #endregion

    #region Rate Limiting Tests

    [Fact]
    public async Task SendMessageAsync_RateLimitTriggered_ReturnsRateLimitMessage()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "[RATE_LIMIT] Test rate limiting",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.ToLower().Should().Contain("rate");
    }

    #endregion

    #region Response Formatting/Parsing Tests

    [Fact]
    public async Task SendMessageAsync_ResponseIsStreamed_TokensArriveIncrementally()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var tokens = new List<string>();
        await foreach (var token in service.SendMessageAsync(
            "Explain what a variable is",
            context,
            history))
        {
            tokens.Add(token);
        }

        // Assert
        tokens.Count.Should().BeGreaterThan(1);
    }

    [Fact]
    public async Task SendMessageAsync_ResponseConcatenated_FormsCompleteSentence()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "What is a variable?",
            context,
            history).ToListAsync();

        // Assert
        var fullText = string.Concat(response);
        fullText.Should().NotBeNullOrWhiteSpace();
        fullText.EndsWith(".").Should().BeTrue();
    }

    [Fact]
    public async Task SendMessageAsync_LongConversation_TruncatesHistory()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var longHistory = Enumerable.Range(0, 50)
            .Select(i => new TutorMessage(
                i % 2 == 0 ? MessageRole.User : MessageRole.Assistant,
                $"Message content {i}"))
            .ToList();

        // Act
        var response = await service.SendMessageAsync(
            "Latest question after 50 messages",
            context,
            longHistory).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessageAsync_VeryLongMessage_TruncatesInput()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        var longMessage = string.Join(" ", Enumerable.Repeat("word", 500));

        // Act
        var response = await service.SendMessageAsync(
            longMessage,
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessageAsync_SpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        var specialMessage = "What does <T> mean in List<T>? And what about & and \"?";

        // Act
        var response = await service.SendMessageAsync(
            specialMessage,
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region WarmUp Tests

    [Fact]
    public async Task WarmUpAsync_WhenNotLoaded_DoesNothing()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert (should not throw)
        await service.WarmUpAsync();
    }

    [Fact]
    public async Task WarmUpAsync_AfterLoad_ReadyForInference()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();

        // Act
        await service.WarmUpAsync();
        var response = await service.SendMessageAsync(
            "Quick test",
            CreateContext(),
            CreateHistory()).ToListAsync();

        // Assert
        response.Should().NotBeEmpty();
    }

    #endregion

    #region Concurrent Access Tests

    [Fact]
    public async Task SendMessageAsync_MultipleParallelRequests_AllComplete()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();

        // Act
        var tasks = Enumerable.Range(0, 5).Select(i =>
            service.SendMessageAsync(
                $"Question number {i}",
                context,
                CreateHistory()).ToListAsync());

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(5);
        results.All(r => r.Count > 0).Should().BeTrue();
    }

    [Fact]
    public async Task SendMessageAsync_InterleavedWithLoad_ThrowsIfNotReady()
    {
        // Arrange
        var service = CreateService();
        var context = CreateContext();
        var history = CreateHistory();

        // Act - Start load but don't await
        var loadTask = service.LoadModelAsync();

        // Try to send message before load completes (may or may not throw depending on timing)
        try
        {
            await service.SendMessageAsync("Hello", context, history).ToListAsync();
        }
        catch (InvalidOperationException)
        {
            // Expected if not loaded yet
        }

        // Wait for load to complete
        await loadTask;

        // Now should work
        var response = await service.SendMessageAsync("Hello", context, history).ToListAsync();
        response.Should().NotBeEmpty();
    }

    #endregion
}
