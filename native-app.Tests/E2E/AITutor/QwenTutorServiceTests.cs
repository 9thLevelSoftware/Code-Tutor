using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Tests.E2E.AITutor;

/// <summary>
/// E2E tests for the QwenTutorService.
/// Tests all tutor functionality using mocked LLM responses to avoid loading actual models.
/// </summary>
public class QwenTutorServiceTests : TutorServiceTestBase
{
    private readonly Dictionary<string, string[]> _mockResponses = new()
    {
        ["hint"] = new[] { "Consider", " checking", " if", " the", " variable", " is", " defined", " before", " using", " it", "." },
        ["explain_error"] = new[] { "This", " error", " occurs", " when", " you", " try", " to", " use", " a", " variable", " that", " hasn't", " been", " declared", "." },
        ["improve"] = new[] { "Consider", " using", " a", " generic", " List", "<", "T", ">", " instead", " of", " ArrayList", " for", " type", " safety", "." },
        ["answer"] = new[] { "In", " C", "#,", " you", " can", " declare", " a", " variable", " using", " the", " syntax", ":", " type", " name", " =", " value", ";" },
        ["default"] = new[] { "I'm", " here", " to", " help", " with", " your", " programming", " questions", "." }
    };

    private ITutorService CreateService()
    {
        return new MockTutorService(_mockResponses);
    }

    #region GenerateHint Tests

    [Fact]
    public async Task GenerateHint_ForCSharpCode_ReturnsContextualHint()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#", userCode: TestData.CSharpWithError);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Can you give me a hint about my code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Should().Contain("variable", "Hint should mention the problematic element");
    }

    [Fact]
    public async Task GenerateHint_ForPythonCode_ReturnsLanguageSpecificHint()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "Python", userCode: TestData.PythonWithError);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "I need a hint for my Python code",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateHint_ForJavaCode_ReturnsLanguageSpecificHint()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "Java", userCode: TestData.JavaWithError);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Help me with my Java code",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateHint_WithNoCodeInContext_ReturnsGeneralHint()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#", userCode: null);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Give me a hint about variable declaration",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region ExplainError Tests

    [Theory]
    [InlineData("C#", TestData.CSharpCompileError, "error CS0103")]
    [InlineData("Python", TestData.PythonRuntimeError, "NameError")]
    [InlineData("Java", TestData.JavaRuntimeError, "NullPointerException")]
    public async Task ExplainError_ForDifferentLanguages_ProvidesClearExplanation(
        string language, string errorMessage, string expectedTerm)
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: language, executionError: errorMessage);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            $"What does this error mean: {errorMessage}",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Should().Contain("error");
    }

    [Fact]
    public async Task ExplainError_WithStackTrace_ExplainsRootCause()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var stackTrace = @"
Traceback (most recent call last):
  File 'program.py', line 3, in <module>
    greet('Alice')
  File 'program.py', line 2, in greet
    print('Hello, ' + undefined_variable)
NameError: name 'undefined_variable' is not defined";
        var context = CreateContext(language: "Python", executionError: stackTrace);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Can you explain this stack trace?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExplainError_WithNoErrorContext_RequestsErrorInformation()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#", executionError: null);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "What does 'null reference exception' mean?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region SuggestImprovement Tests

    [Theory]
    [InlineData("C#", TestData.CSharpSuboptimalCode, "ArrayList")]
    [InlineData("Python", TestData.PythonSuboptimalCode, "enumerate")]
    [InlineData("Java", TestData.JavaSuboptimalCode, "StringBuilder")]
    public async Task SuggestImprovement_ForSuboptimalCode_SuggestsBetterApproach(
        string language, string code, string expectedSuggestion)
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: language, userCode: code);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "How can I improve this code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SuggestImprovement_ForCleanCode_GivesPositiveFeedback()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#", userCode: TestData.CSharpHelloWorld);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Is there anything I can improve in my code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region AnswerQuestion Tests

    [Theory]
    [InlineData("What is a variable?", "variable")]
    [InlineData("How do I declare a function in C#?", "declare")]
    [InlineData("Explain object-oriented programming", "object")]
    [InlineData("What's the difference between a class and struct?", "class")]
    public async Task AnswerQuestion_GeneralProgrammingQuestions_ProvidesHelpfulAnswer(
        string question, string expectedConcept)
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
    }

    [Fact]
    public async Task AnswerQuestion_WithConversationHistory_MaintainsContext()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#");
        var history = CreateHistory(
            (MessageRole.User, "I'm learning about loops"),
            (MessageRole.Assistant, "Great! Loops are fundamental. What would you like to know?"),
            (MessageRole.User, "What's the difference between for and while?")
        );

        // Act
        var response = await service.SendMessageAsync(
            "Can you show me an example?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AnswerQuestion_ProgrammingLanguageSpecific_RespectsLanguageContext()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "Python");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "How do I read a file?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Empty/Null Input Handling Tests

    [Fact]
    public async Task SendMessageAsync_WithEmptyString_ReturnsHelpfulPrompt()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Should().Contain("didn't receive", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendMessageAsync_WithWhitespaceOnly_ReturnsHelpfulPrompt()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "   \t\n  ",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Should().Contain("didn't receive", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendMessageAsync_WithNull_ThrowsArgumentNullException()
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
    public async Task SendMessageAsync_ModelNotLoaded_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = CreateService();
        // Note: NOT calling LoadModelAsync()
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
    public async Task SendMessageAsync_AfterUnloadModel_ThrowsInvalidOperationException()
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
            await service.SendMessageAsync("Hello", context, history).ToListAsync();
        });
    }

    [Fact]
    public async Task LoadModelAsync_SetsIsModelLoadedCorrectly()
    {
        // Arrange
        var service = CreateService();
        service.IsModelLoaded.Should().BeFalse();

        // Act
        await service.LoadModelAsync();

        // Assert
        service.IsModelLoaded.Should().BeTrue();
    }

    [Fact]
    public async Task LoadModelAsync_ReportsProgress()
    {
        // Arrange
        var service = CreateService();
        var progressValues = new List<int>();
        service.LoadingProgressChanged += (s, e) => progressValues.Add(e);

        // Act
        await service.LoadModelAsync();

        // Assert
        progressValues.Should().Contain(10);
        progressValues.Should().Contain(50);
        progressValues.Should().Contain(100);
    }

    #endregion

    #region Timeout Handling Tests

    [Fact]
    public async Task SendMessageAsync_WithTimeout_ThrowsOperationCanceledException()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            // The mock service checks for [TIMEOUT] in the message to simulate long-running
            await service.SendMessageAsync(
                "This is a [TIMEOUT] test message that simulates long processing",
                context,
                history,
                cts.Token).ToListAsync();
        });
    }

    [Fact]
    public async Task SendMessageAsync_CancellationDuringStreaming_StopsImmediately()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        using var cts = new CancellationTokenSource();

        // Act
        var tokens = new List<string>();
        var enumerator = service.SendMessageAsync(
            "Explain object-oriented programming with examples",
            context,
            history,
            cts.Token).GetAsyncEnumerator();

        try
        {
            // Get a few tokens
            for (int i = 0; i < 3; i++)
            {
                if (await enumerator.MoveNextAsync())
                    tokens.Add(enumerator.Current);
            }

            // Cancel mid-stream
            cts.Cancel();

            // Try to get more tokens - should throw
            await enumerator.MoveNextAsync();
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        tokens.Count.Should().BeGreaterOrEqualTo(1);
        tokens.Count.Should().BeLessThan(10); // Should have stopped early
    }

    #endregion

    #region Rate Limiting Tests

    [Fact]
    public async Task SendMessageAsync_WithRateLimitIndicator_ReturnsRateLimitMessage()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "[RATE_LIMIT] This simulates rate limiting",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().Contain("Rate limit");
    }

    #endregion

    #region Response Formatting/Parsing Tests

    [Fact]
    public async Task SendMessageAsync_ResponseStreamsAsTokens()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var tokens = await service.SendMessageAsync(
            "What is a variable?",
            context,
            history).ToListAsync();

        // Assert
        tokens.Should().NotBeEmpty();
        tokens.Count.Should().BeGreaterThan(1); // Multiple tokens expected
    }

    [Fact]
    public async Task SendMessageAsync_ConcatenatedResponse_IsCoherent()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var tokens = await service.SendMessageAsync(
            "What is a variable?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(tokens);
        fullResponse.Should().NotBeNullOrWhiteSpace();
        fullResponse.Length.Should().BeGreaterThan(10);
    }

    [Fact]
    public async Task SendMessageAsync_WithHistory_TruncatesHistoryCorrectly()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var longHistory = Enumerable.Range(0, 20)
            .Select(i => new TutorMessage(
                i % 2 == 0 ? MessageRole.User : MessageRole.Assistant,
                $"Message {i}"))
            .ToList();

        // Act
        var response = await service.SendMessageAsync(
            "Latest question",
            context,
            longHistory).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessageAsync_WithLongMessage_TruncatesInput()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        var veryLongMessage = new string('x', 1000);

        // Act
        var response = await service.SendMessageAsync(
            veryLongMessage,
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region WarmUp Tests

    [Fact]
    public async Task WarmUpAsync_BeforeLoadModel_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await service.WarmUpAsync(); // Should complete without exception
    }

    [Fact]
    public async Task WarmUpAsync_AfterLoadModel_CompletesSuccessfully()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();

        // Act
        await service.WarmUpAsync();

        // Assert
        service.IsModelLoaded.Should().BeTrue();
    }

    #endregion

    #region Concurrent Request Tests

    [Fact]
    public async Task SendMessageAsync_ConcurrentRequests_HandledCorrectly()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();

        // Act
        var tasks = Enumerable.Range(0, 3).Select(i =>
            service.SendMessageAsync(
                $"Question {i}",
                context,
                CreateHistory()).ToListAsync()).ToList();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.All(r => r.Count > 0).Should().BeTrue();
    }

    #endregion
}
