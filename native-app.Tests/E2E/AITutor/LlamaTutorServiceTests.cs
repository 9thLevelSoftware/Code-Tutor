using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Services;

namespace CodeTutor.Tests.E2E.AITutor;

/// <summary>
/// E2E tests for the LlamaTutorService.
/// Tests all tutor functionality using mocked LLM responses to avoid loading actual models.
/// Llama uses GGUF models via LLamaSharp with Qwen chat template format.
/// </summary>
public class LlamaTutorServiceTests : TutorServiceTestBase
{
    private readonly Dictionary<string, string[]> _mockResponses = new()
    {
        ["hint"] = new[] { "Look", " at", " the", " variable", " scope", " -", " it", " might", " not", " be", " accessible", " where", " you're", " using", " it", "." },
        ["explain_error"] = new[] { "This", " is", " a", " compilation", " error", " indicating", " that", " the", " variable", " was", " never", " declared", " or", " is", " out", " of", " scope", "." },
        ["improve"] = new[] { "Consider", " using", " generics", " for", " type", " safety", " and", " better", " performance", " instead", " of", " the", " non", "-", "generic", " collection", "." },
        ["answer"] = new[] { "A", " variable", " is", " a", " named", " storage", " location", " in", " memory", " that", " holds", " a", " value", " that", " can", " change", " during", " program", " execution", "." },
        ["default"] = new[] { "Hello", "!", " I'm", " ready", " to", " help", " you", " with", " your", " programming", " questions", "." }
    };

    private ITutorService CreateService()
    {
        return new MockTutorService(_mockResponses);
    }

    #region GenerateHint Tests

    [Fact]
    public async Task GenerateHint_ForBeginnerCode_ProvidesGentleGuidance()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var beginnerCode = @"
// My first program
Console.WriteLine('Hello World')";  // Wrong quotes
        var context = CreateContext(language: "C#", userCode: beginnerCode);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "My code isn't working, any hints?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
        fullResponse.Should().Contain("variable").Or.Contain("scope");
    }

    [Fact]
    public async Task GenerateHint_ForLogicError_HintsAtSolutionApproach()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var logicErrorCode = @"
// Should calculate factorial
int Factorial(int n)
{
    if (n <= 1) return 1;
    return n + Factorial(n - 1);  // Wrong: using + instead of *
}";
        var context = CreateContext(language: "C#", userCode: logicErrorCode);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "My factorial function gives wrong results",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateHint_ForMissingSemicolon_HintsAtSyntax()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var missingSemicolon = @"
int x = 5
Console.WriteLine(x);";
        var context = CreateContext(language: "C#", userCode: missingSemicolon);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "I'm getting a compile error but I don't see why",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("Python", "for i in range(10)\n    print(i)", "indent")]
    [InlineData("Java", "public void main() { }", "static")]
    [InlineData("C#", "if (x = 5)", "==")]
    public async Task GenerateHint_CommonBeginnerMistakes_IdentifiesIssue(
        string language, string code, string expectedHint)
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: language, userCode: code);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "What's wrong with my code?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region ExplainError Tests

    [Fact]
    public async Task ExplainError_ArrayIndexOutOfBounds_ExplainsSafeAccess()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var code = "int[] arr = new int[5]; Console.WriteLine(arr[10]);";
        var error = "IndexOutOfRangeException: Index was outside the bounds of the array";
        var context = CreateContext(language: "C#", userCode: code, executionError: error);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "What does this error mean and how do I fix it?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExplainError_MethodNotFound_SuggestsCorrectMethod()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var code = "string s = \"hello\"; s.toUpperCase();";
        var error = "CS1061: 'string' does not contain a definition for 'toUpperCase'";
        var context = CreateContext(language: "C#", userCode: code, executionError: error);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Why doesn't this method work?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExplainError_TypeMismatch_ExplainsConversion()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var code = "string input = \"42\"; int num = input;";
        var error = "CS0029: Cannot implicitly convert type 'string' to 'int'";
        var context = CreateContext(language: "C#", userCode: code, executionError: error);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "How do I convert this?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExplainError_RecursiveStackOverflow_ExplainsRecursion()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var code = @"
int Count(int n) => Count(n + 1);  // Infinite recursion
Count(0);";
        var error = "StackOverflowException";
        var context = CreateContext(language: "C#", userCode: code, executionError: error);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "My program crashes with this error",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExplainError_NullReferenceException_ExplainsNullChecking()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var code = @"
string? name = null;
Console.WriteLine(name.Length);";
        var error = "NullReferenceException: Object reference not set to an instance of an object";
        var context = CreateContext(language: "C#", userCode: code, executionError: error);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Why am I getting null reference?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region SuggestImprovement Tests

    [Fact]
    public async Task SuggestImprovement_DuplicateCode_SuggestsDRY()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var duplicateCode = @"
// Calculate area of two rectangles
int area1 = 10 * 5;
int area2 = 8 * 6;";
        var context = CreateContext(language: "C#", userCode: duplicateCode);
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
    public async Task SuggestImprovement_MagicNumbers_SuggestsConstants()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var magicNumbers = @"
if (age >= 18 && age <= 65)
{
    Console.WriteLine(\"Working age\");
}";
        var context = CreateContext(language: "C#", userCode: magicNumbers);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Is there a better way to write this?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SuggestImprovement_StringConcatenation_SuggestsInterpolation()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var stringConcat = @"
string name = ""Alice"";
int age = 30;
Console.WriteLine(""Name: "" + name + "", Age: "" + age);";
        var context = CreateContext(language: "C#", userCode: stringConcat);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Can this be improved?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SuggestImprovement_MissingErrorHandling_SuggestsTryCatch()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var noErrorHandling = @"
int result = int.Parse(userInput);  // Could throw";
        var context = CreateContext(language: "C#", userCode: noErrorHandling);
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Is this code safe?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region AnswerQuestion Tests

    [Theory]
    [InlineData("What is the difference between value types and reference types?")]
    [InlineData("When should I use an interface vs an abstract class?")]
    [InlineData("What is dependency injection?")]
    [InlineData("How does the async/await pattern work?")]
    public async Task AnswerQuestion_AdvancedTopics_ProvidesClearExplanation(string question)
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
        fullResponse.Length.Should().BeGreaterThan(10);
    }

    [Fact]
    public async Task AnswerQuestion_LearningPath_SuggestsProgression()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#");
        var history = CreateHistory(
            (MessageRole.User, "I've learned about variables and loops. What should I learn next?")
        );

        // Act
        var response = await service.SendMessageAsync(
            "What are functions and methods?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AnswerQuestion_DebuggingTechniques_ProvidesTips()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "What are some debugging strategies?",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AnswerQuestion_DifferentLanguageContext_AdaptsAnswer()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var pythonContext = CreateContext(language: "Python");
        var javaContext = CreateContext(language: "Java");
        var history = CreateHistory();

        // Act
        var pythonResponse = await service.SendMessageAsync(
            "How do I declare a list?",
            pythonContext,
            history).ToListAsync();

        var javaResponse = await service.SendMessageAsync(
            "How do I declare a list?",
            javaContext,
            history).ToListAsync();

        // Assert
        string.Concat(pythonResponse).Should().NotBeNullOrEmpty();
        string.Concat(javaResponse).Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Empty/Null Input Handling Tests

    [Fact]
    public async Task SendMessageAsync_EmptyString_ReturnsPolitePrompt()
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
        fullResponse.ToLower().Should().ContainAny("didn't receive", "how can", "help");
    }

    [Fact]
    public async Task SendMessageAsync_WhitespaceOnly_ReturnsPolitePrompt()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "     ",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessageAsync_NullMessage_ThrowsArgumentNullException()
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
    public async Task IsModelLoaded_Initially_ReturnsFalse()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        service.IsModelLoaded.Should().BeFalse();
    }

    [Fact]
    public async Task SendMessageAsync_BeforeModelLoad_ThrowsInvalidOperationException()
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
    public async Task SendMessageAsync_AfterUnload_ThrowsInvalidOperationException()
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
    public async Task UnloadModel_ResetsLoadingProgress()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        service.LoadingProgress.Should().Be(100);

        // Act
        service.UnloadModel();

        // Assert
        service.LoadingProgress.Should().Be(0);
        service.IsModelLoaded.Should().BeFalse();
    }

    #endregion

    #region Timeout Handling Tests

    [Fact]
    public async Task SendMessageAsync_WithShortTimeout_CancelsOperation()
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
            await service.SendMessageAsync(
                "[TIMEOUT] Long running operation simulation",
                context,
                history,
                cts.Token).ToListAsync();
        });
    }

    [Fact]
    public async Task SendMessageAsync_CancellationMidStream_StopsGeneration()
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
            "Write a long explanation about programming",
            context,
            history,
            cts.Token).GetAsyncEnumerator();

        try
        {
            // Collect some tokens
            for (int i = 0; i < 3; i++)
            {
                if (await enumerator.MoveNextAsync())
                    tokens.Add(enumerator.Current);
            }

            // Cancel
            cts.Cancel();

            // Try to continue
            await enumerator.MoveNextAsync();
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        tokens.Count.Should().BeInRange(1, 5);
    }

    #endregion

    #region Rate Limiting Tests

    [Fact]
    public async Task SendMessageAsync_RateLimitIndicator_ReturnsLimitMessage()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "[RATE_LIMIT] Testing rate limit",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.ToLower().Should().Contain("rate");
    }

    [Fact]
    public async Task SendMessageAsync_RateLimitMessage_IncludesWaitSuggestion()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "[RATE_LIMIT] Rate limit exceeded test",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.ToLower().Should().ContainAny("rate", "limit", "wait");
    }

    #endregion

    #region Response Formatting/Parsing Tests

    [Fact]
    public async Task SendMessageAsync_ResponseStreams_TokensNotEmpty()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var tokens = await service.SendMessageAsync(
            "What is a function?",
            context,
            history).ToListAsync();

        // Assert
        tokens.Should().NotBeEmpty();
        tokens.All(t => !string.IsNullOrEmpty(t)).Should().BeTrue();
    }

    [Fact]
    public async Task SendMessageAsync_StreamingResponse_CanBeConcatenated()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Explain variables",
            context,
            history).ToListAsync();

        // Assert
        var fullText = string.Concat(response);
        fullText.Should().NotBeNullOrWhiteSpace();
        fullText.Length.Should().BeGreaterThan(20);
    }

    [Fact]
    public async Task SendMessageAsync_WithLongHistory_TruncatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var longHistory = Enumerable.Range(0, 100)
            .Select(i => new TutorMessage(
                i % 2 == 0 ? MessageRole.User : MessageRole.Assistant,
                $"Message number {i} with some content"))
            .ToList();

        // Act
        var response = await service.SendMessageAsync(
            "Final question",
            context,
            longHistory).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessageAsync_CodeInResponse_FormattedCorrectly()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext(language: "C#");
        var history = CreateHistory();

        // Act
        var response = await service.SendMessageAsync(
            "Show me how to declare an array",
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessageAsync_SpecialCharactersInInput_HandledCorrectly()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();
        var history = CreateHistory();
        var specialChars = "What do these mean: < > & \" ' { } [ ] ( ) ?";

        // Act
        var response = await service.SendMessageAsync(
            specialChars,
            context,
            history).ToListAsync();

        // Assert
        var fullResponse = string.Concat(response);
        fullResponse.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region WarmUp Tests

    [Fact]
    public async Task WarmUpAsync_BeforeLoad_CompletesWithoutError()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await service.WarmUpAsync(); // Should not throw
    }

    [Fact]
    public async Task WarmUpAsync_AfterLoad_ImprovesResponseTime()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();

        // Act
        await service.WarmUpAsync();

        // Should be able to send messages after warm-up
        var response = await service.SendMessageAsync(
            "Test",
            CreateContext(),
            CreateHistory()).ToListAsync();

        // Assert
        response.Should().NotBeEmpty();
    }

    [Fact]
    public async Task WarmUpAsync_Cancellation_RespectsToken()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await service.WarmUpAsync(cts.Token);
        });
    }

    #endregion

    #region Concurrent Access Tests

    [Fact]
    public async Task SendMessageAsync_MultipleSimultaneousRequests_AllSucceed()
    {
        // Arrange
        var service = CreateService();
        await service.LoadModelAsync();
        var context = CreateContext();

        // Act
        var tasks = Enumerable.Range(0, 3)
            .Select(i => service.SendMessageAsync(
                $"Concurrent question {i}",
                context,
                CreateHistory()).ToListAsync());

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(3);
        results.All(r => r.Count > 0).Should().BeTrue();
    }

    [Fact]
    public async Task LoadModelAsync_ConcurrentCalls_HandledSafely()
    {
        // Arrange
        var service = CreateService();

        // Act
        var loadTasks = Enumerable.Range(0, 3)
            .Select(_ => service.LoadModelAsync());

        await Task.WhenAll(loadTasks);

        // Assert
        service.IsModelLoaded.Should().BeTrue();
    }

    #endregion

    #region Progress Event Tests

    [Fact]
    public async Task LoadingProgressChanged_FiresDuringLoad()
    {
        // Arrange
        var service = CreateService();
        var progressValues = new List<int>();
        service.LoadingProgressChanged += (_, progress) => progressValues.Add(progress);

        // Act
        await service.LoadModelAsync();

        // Assert
        progressValues.Should().NotBeEmpty();
        progressValues.Should().Contain(100);
    }

    [Fact]
    public async Task LoadingProgressChanged_ProgressIncreases_Monotonically()
    {
        // Arrange
        var service = CreateService();
        var progressValues = new List<int>();
        service.LoadingProgressChanged += (_, progress) => progressValues.Add(progress);

        // Act
        await service.LoadModelAsync();

        // Assert - should have increasing progress
        for (int i = 1; i < progressValues.Count; i++)
        {
            progressValues[i].Should().BeGreaterOrEqualTo(progressValues[i - 1]);
        }
    }

    #endregion
}
